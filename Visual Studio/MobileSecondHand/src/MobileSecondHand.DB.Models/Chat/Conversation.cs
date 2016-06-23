using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.DB.Models.Chat
{
	public class Conversation
	{
		public int ConversationId { get; set; }
		public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
		public List<UserToConversation> Users { get; set; } = new List<UserToConversation>();
	}
}
