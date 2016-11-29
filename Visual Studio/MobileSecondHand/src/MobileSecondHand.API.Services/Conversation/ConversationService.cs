using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Chat;
using MobileSecondHand.API.Models.Config;
using MobileSecondHand.API.Models.Shared.Chat;
using MobileSecondHand.API.Services.Authentication;
using MobileSecondHand.API.Services.Photos;
using MobileSecondHand.COMMON.Extensions;
using MobileSecondHand.DB.Models.Chat;
using MobileSecondHand.DB.Services.Chat;

namespace MobileSecondHand.API.Services.Conversation
{
	public class ConversationService : IConversationService
	{
		IConversationDbService conversationDbService;
		IPhotosService photosService;
		IApplicationUserManager applicationUserManager;
		AppConfiguration appConfiguration;

		public ConversationService(IConversationDbService conversationDbService, IPhotosService photosService, IApplicationUserManager applicationUserManager, AppConfiguration appConfiguration)
		{
			this.conversationDbService = conversationDbService;
			this.photosService = photosService;
			this.applicationUserManager = applicationUserManager;
			this.appConfiguration = appConfiguration;
		}

		public async Task<bool> SendHelloMessageToNewUser(string addresseeId)
		{
			var isSent = false;
			var sender = await applicationUserManager.GetUserByEmail(appConfiguration.AutomaticChatMessagesSenderEmail);

			if (sender != null)
			{
				var conversation = await GetConversationInfoModel(sender.Id, addresseeId);
				var helloMessage = new ChatMessageSaveModel
				{
					ConversationId = conversation.ConversationId,
					SenderId = sender.Id,
					AddresseeId = addresseeId,
					Content = Properties.Resources.helloMSH
				};

				AddMessageToConversation(helloMessage);
				isSent = true;
			}

			return isSent;
		}

		public bool DeleteConversation(string userId, int conversationId)
		{
			var conversation = this.conversationDbService.GetByIdWithUsers(conversationId);
			var userToConversation = conversation.Users.First(u => u.UserId == userId);
			userToConversation.Deleted = true;

			this.conversationDbService.SaveUserToConversation(userToConversation);

			return true;
		}

		public List<ChatMessageReadModel> GetMessages(string userId, int conversationId, int pageNumber)
		{
			var messagesViewModelList = new List<ChatMessageReadModel>();
			List<ChatMessage> messagesDbList = this.conversationDbService.GetMessagesInConversation(conversationId, pageNumber).ToList();
			var notReceivedMessagesIds = messagesDbList.Where(m => m.AuthorId != userId && !m.Received).ToList();
			this.conversationDbService.UpdateReceivedPropertyInMessages(notReceivedMessagesIds);

			foreach (var message in messagesDbList)
			{
				messagesViewModelList.Add(MapChatMessageToReadModel(userId, message));
			}

			return messagesViewModelList;
		}


		/// <summary>
		/// Returns conversation Id in db or create new if doesn't exist;
		/// </summary>
		/// <param name="userId">Id of user who is message sender</param>
		/// <param name="addresseeId">Id of user who will be message addressee</param>
		/// <returns>Id of conversation record in db</returns>
		public async Task<ConversationInfoModel> GetConversationInfoModel(string userId, string addresseeId)
		{
			var result = new ConversationInfoModel();
			if (userId == addresseeId)
			{
				//sender and addressee is the same user
				return result;
			}

			var conversation = this.conversationDbService.GetConversationByUsers(userId, addresseeId);
			if (conversation == null)
			{
				this.conversationDbService.CreateConversation(userId, addresseeId);
				conversation = this.conversationDbService.GetConversationByUsers(userId, addresseeId);
			}

			result.ConversationId = conversation.ConversationId;
			result.InterlocutorId = addresseeId;
			result.InterlocutorName = conversation.Users.First(u => u.UserId != userId).User.UserName;
			result.InterlocutorPrifileImage = await this.photosService.GetUserProfilePhotoInBytes(conversation.Users.First(u => u.UserId != userId).User.UserProfilePhotoName);

			return result;
		}

		/// <summary>
		/// Saves message in db and returns message read model
		/// </summary>
		/// <param name="chatMessageSaveModel">Model with message parameters</param>
		/// <returns>Message read model</returns>
		public ChatMessageReadModel AddMessageToConversation(ChatMessageSaveModel chatMessageSaveModel)
		{
			UpdateConversationIfIsMarkedAsDeleted(chatMessageSaveModel.ConversationId);

			var messageDbModel = new ChatMessage();
			messageDbModel.AuthorId = chatMessageSaveModel.SenderId;
			messageDbModel.ConversationId = chatMessageSaveModel.ConversationId;
			messageDbModel.Content = chatMessageSaveModel.Content;
			messageDbModel.Date = DateTime.Now;

			messageDbModel = this.conversationDbService.SaveMessage(messageDbModel);

			return MapChatMessageToReadModel(chatMessageSaveModel.AddresseeId, messageDbModel);
		}

		private void UpdateConversationIfIsMarkedAsDeleted(int conversationId)
		{
			var conversation = this.conversationDbService.GetByIdWithUsers(conversationId);
			if (conversation.Users.Any(c => c.Deleted))
			{
				foreach (var user in conversation.Users)
				{
					if (user.Deleted)
					{
						user.Deleted = false;
						this.conversationDbService.SaveUserToConversation(user);
					}
				}
			}
		}

		public void MarkMessageAsReceived(int messageId)
		{
			this.conversationDbService.MarkMessageAsReceived(messageId);
		}

		public IEnumerable<ChatMessageReadModel> GetNotReceivedMessagesAndMarkThemReceived(string userId)
		{
			var result = new List<ChatMessageReadModel>();
			var notReceivedMessagesDictionary = this.conversationDbService.GetNotReceivedMessagesDictionary(userId);

			if (notReceivedMessagesDictionary.Count == 0)
			{
				//nothing not received
				return result;
			}

			foreach (var item in notReceivedMessagesDictionary)
			{
				result.Add(MapChatMessageToReadModel(userId, item.Value));
			}

			//marking received
			this.conversationDbService.UpdateReceivedPropertyInNotReceivedMessages(userId, notReceivedMessagesDictionary.Select(k => k.Key));

			return result;
		}

		public async Task<List<ConversationItemModel>> GetConversations(string userId, int pageNumber)
		{
			var conversations = this.conversationDbService.GetConversationsWithLastMessage(userId, pageNumber);
			List<ConversationItemModel> coversationViewModelList = await MapToViewModels(userId, conversations);

			return coversationViewModelList;
		}

		private async Task<List<ConversationItemModel>> MapToViewModels(string userId, List<DB.Models.Chat.ConversationReadModel> conversations)
		{
			var list = new List<ConversationItemModel>();

			foreach (var conversation in conversations)
			{
				var model = new ConversationItemModel();
				model.Id = conversation.ConversationId;
				model.InterLocutorId = conversation.Users.First(u => u.Id != userId).Id;
				model.InterlocutorName = conversation.Users.First(u => u.Id != userId).UserName;
				model.LastMessage = conversation.Messages[0].Content;
				model.LastMessageDate = GetDateString(conversation.Messages[0].Date);
				model.InterLocutorProfileImage = await this.photosService.GetUserProfilePhotoInBytes(conversation.Users.First(u => u.Id != userId).UserProfilePhotoName);

				list.Add(model);
			}

			return list;
		}

		private ChatMessageReadModel MapChatMessageToReadModel(string userId, ChatMessage message)
		{
			var messageViewModel = new ChatMessageReadModel();
			messageViewModel.Id = message.ChatMessageId;
			messageViewModel.ConversationId = message.ConversationId;
			messageViewModel.MessageHeader = GetMessageHeader(userId, message);
			messageViewModel.MessageContent = message.Content;
			messageViewModel.UserWasSender = userId == message.AuthorId;
			messageViewModel.SenderId = message.AuthorId;
			messageViewModel.SenderName = message.Author.UserName;
			return messageViewModel;
		}

		private string GetMessageHeader(string userId, ChatMessage message)
		{
			return string.Format("{0}, {1} {2}", userId == message.AuthorId ? "ja" : message.Author.UserName, GetDateString(message.Date), message.Date.GetTimeColonStringFormat());
		}

		private string GetDateString(DateTime date)
		{
			var date1 = new DateTime(date.Year, date.Month, date.Day);
			var dateNow = DateTime.Now;
			var date2 = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day);

			if (date1 == date2)
			{
				return "dzisiaj";

			}
			else if (date1.AddDays(1) == date2)
			{
				return "wczoraj";
			}

			return date.GetDateDottedStringFormat();
		}
	}

}
