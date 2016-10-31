using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Chat;
using MobileSecondHand.Models.Chat;

namespace MobileSecondHand.Services.Chat
{
	public interface IMessagesService {
		Task<List<ConversationMessage>> GetMessages(int conversationId, int pageNumber);
		Task<ConversationInfoModel> GetConversationInfoModel(string addresseeId);
		Task<List<ConversationItemModel>> GetConversations(int pageNumber);
	}
}
