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

		public List<ChatMessageViewModel> GetMessages(string userId, int conversationId, int pageNumber)
		{
			var messagesViewModelList = new List<ChatMessageViewModel>();
			List<ChatMessage> messagesDbList = this.conversationDbService.GetMessagesInConversation(conversationId, pageNumber);

			foreach (var message in messagesDbList)
			{
				var messageViewModel = new ChatMessageViewModel();
				messageViewModel.Id = message.ChatMessageId;
				messageViewModel.ConversationId = message.ConversationId;
				messageViewModel.MessageHeader = GetMessageHeader(userId, message);
				messageViewModel.MessageContent = message.Content;
				messageViewModel.UserWasSender = userId == message.AuthorId;

				messagesViewModelList.Add(messageViewModel);
			}

			return messagesViewModelList;
		}

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

		private string GetMessageHeader(string userId, ChatMessage message)
		{
			return String.Format("{0}, {1} {2}", userId == message.AuthorId ? "ja" : message.Author.GetUserName(), message.Date.GetDateDottedStringFormat(), message.Date.GetTimeColonStringFormat());
		}
	}
}
