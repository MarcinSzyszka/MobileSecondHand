using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Chat;
using MobileSecondHand.API.Models.Shared.Chat;

namespace MobileSecondHand.API.Services.Conversation
{
	public interface IConversationService {
		List<ChatMessageReadModel> GetMessages(string userId, int conversationId, int pageNumber);
		Task<ConversationInfoModel> GetConversationInfoModel(string userId, string addresseeId);
		ChatMessageReadModel AddMessageToConversation(ChatMessageSaveModel chatMessageSaveModel);
		IEnumerable<ChatMessageReadModel> GetNotReceivedMessagesAndMarkThemReceived(string userId);
		void MarkMessageAsReceived(int messageId);
		Task<List<ConversationItemModel>> GetConversations(string userId, int pageNumber);
		bool DeleteConversation(string userId, int conversationId);
	}
}
