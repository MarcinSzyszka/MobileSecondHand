using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Authentication;
using MobileSecondHand.DB.Models.Chat;

namespace MobileSecondHand.DB.Services.Chat
{
	public interface IConversationDbService
	{
		IEnumerable<ChatMessage> GetMessagesInConversation(int conversationId, int pageNumber);
		Conversation GetConversationByUsers(string userId, string addresseeId);
		Conversation CreateConversation(string userId, string addresseeId);
		ChatMessage SaveMessage(ChatMessage messageDbModel);
		IDictionary<int, ChatMessage> GetNotReceivedMessagesDictionary(string userId);
		void UpdateReceivedPropertyInNotReceivedMessages(string userId, IEnumerable<int> conversationsIds);
		void UpdateReceivedPropertyInMessages(List<ChatMessage> messages);
		void MarkMessageAsReceived(int messageId);
		List<ConversationReadModel> GetConversationsWithLastMessage(string userId, int pageNumber);
	}
}
