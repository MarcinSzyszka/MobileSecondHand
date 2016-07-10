using System;
using System.Collections.Generic;
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
		public int GetConversationId(string userId, string addresseeId)
		{
			if (userId == addresseeId)
			{
				//sender and addressee is the same user
				return 0;
			}

			var conversation = this.conversationDbService.GetConversationByUsers(userId, addresseeId);
			if (conversation == null)
			{
				conversation = this.conversationDbService.CreateConversation(userId, addresseeId);
			}

			return conversation.ConversationId;
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
			messageDbModel.Received = chatMessageSaveModel.AddresseeCanReceiveMessage;

			messageDbModel = this.conversationDbService.SaveMessage(messageDbModel);

			return MapChatMessageToReadModel(chatMessageSaveModel.AddresseeId, messageDbModel);
		}

		private ChatMessageReadModel MapChatMessageToReadModel(string userId, ChatMessage message)
		{
			var messageViewModel = new ChatMessageReadModel();
			messageViewModel.Id = message.ChatMessageId;
			messageViewModel.ConversationId = message.ConversationId;
			messageViewModel.MessageHeader = GetMessageHeader(userId, message);
			messageViewModel.MessageContent = message.Content;
			messageViewModel.UserWasSender = userId == message.AuthorId;
			return messageViewModel;
		}

		private string GetMessageHeader(string userId, ChatMessage message)
		{
			return String.Format("{0}, {1} {2}", userId == message.AuthorId ? "ja" : message.Author.GetUserName(), message.Date.GetDateDottedStringFormat(), message.Date.GetTimeColonStringFormat());
		}
	}
}
