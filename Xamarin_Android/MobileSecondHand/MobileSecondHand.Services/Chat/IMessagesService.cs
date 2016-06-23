using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Chat;

namespace MobileSecondHand.Services.Chat {
	public interface IMessagesService {
		Task<List<ConversationMessage>> GetMessages(int conversationId, int pageNumber, string bearerToken);
	}
}
