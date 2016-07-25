using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.Models.Security;

namespace MobileSecondHand.Services.Chat {
	public interface IMessagesService {
		Task<List<ConversationMessage>> GetMessages(int conversationId, int pageNumber);
		Task<ConversationInfoModel> GetConversationInfoModel(string addresseeId);
		Task<List<ConversationItemModel>> GetConversations(int pageNumber);
	}
}
