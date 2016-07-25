using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Chat;
using MobileSecondHand.API.Services.Authentication;
using MobileSecondHand.COMMON.Extensions;
using MobileSecondHand.DB.Models.Chat;
using MobileSecondHand.DB.Services.Chat;

namespace MobileSecondHand.API.Services.Conversation
{
	public class ConversationService : IConversationService
	{
		IConversationDbService conversationDbService;

		public ConversationService(IConversationDbService conversationDbService)
		{
			this.conversationDbService = conversationDbService;
		}

		public List<ChatMessageReadModel> GetMessages(string userId, int conversationId, int pageNumber)
		{
			var messagesViewModelList = new List<ChatMessageReadModel>();
			List<ChatMessage> messagesDbList = this.conversationDbService.GetMessagesInConversation(conversationId, pageNumber);

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
		public ConversationInfoModel GetConversationInfoModel(string userId, string addresseeId)
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
			result.InterlocutorName = conversation.Users.First(u => u.UserId != userId).User.GetUserName();

			return result;
		}

		/// <summary>
		/// Saves message in db and returns message read model
		/// </summary>
		/// <param name="chatMessageSaveModel">Model with message parameters</param>
		/// <returns>Message read model</returns>
		public ChatMessageReadModel AddMessageToConversation(ChatMessageSaveModel chatMessageSaveModel)
		{
			var messageDbModel = new ChatMessage();
			messageDbModel.AuthorId = chatMessageSaveModel.SenderId;
			messageDbModel.Content = chatMessageSaveModel.Content;
			messageDbModel.ConversationId = chatMessageSaveModel.ConversationId;
			messageDbModel.Date = DateTime.Now;

			messageDbModel = this.conversationDbService.SaveMessage(messageDbModel);

			return MapChatMessageToReadModel(chatMessageSaveModel.AddresseeId, messageDbModel);
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

		public List<ConversationItemModel> GetConversations(string userId, int pageNumber)
		{
			var conversations = this.conversationDbService.GetConversationsWithLastMessage(userId, pageNumber);
			List<ConversationItemModel> coversationViewModelList = MapToViewModels(userId, conversations);

			return coversationViewModelList;
		}

		private List<ConversationItemModel> MapToViewModels(string userId, List<DB.Models.Chat.ConversationReadModel> conversations)
		{
			var list = new List<ConversationItemModel>();

			foreach (var conversation in conversations)
			{
				var model = new ConversationItemModel();
				model.Id = conversation.ConversationId;
				model.InterLocutorId = conversation.Users.First(u => u.Id != userId).Id;
				model.InterlocutorName = conversation.Users.First(u => u.Id != userId).GetUserName();
				model.LastMessage = conversation.Messages[0].Content;
				model.LastMessageDate = GetDateString(conversation.Messages[0].Date);

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
			messageViewModel.SenderName = message.Author.GetUserName();
			return messageViewModel;
		}

		private string GetMessageHeader(string userId, ChatMessage message)
		{
			return string.Format("{0}, {1} {2}", userId == message.AuthorId ? "ja" : message.Author.GetUserName(), GetDateString(message.Date), message.Date.GetTimeColonStringFormat());
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
