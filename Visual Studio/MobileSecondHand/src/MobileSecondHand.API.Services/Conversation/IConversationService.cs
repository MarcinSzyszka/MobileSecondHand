using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Chat;

namespace MobileSecondHand.API.Services.Conversation {
	public interface IConversationService {
		List<ChatMessageReadModel> GetMessages(string userId, int conversationId, int pageNumber);
		ConversationInfoModel GetConversationInfoModel(string userId, string addresseeId);
		ChatMessageReadModel AddMessageToConversation(ChatMessageSaveModel chatMessageSaveModel);
		IEnumerable<ChatMessageReadModel> GetNotReceivedMessagesAndMarkThemReceived(string userId);
		void MarkMessageAsReceived(int messageId);
	}
}
