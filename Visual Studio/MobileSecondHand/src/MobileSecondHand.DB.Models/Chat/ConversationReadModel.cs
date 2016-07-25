using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.DB.Models.Chat
{
	public class ConversationReadModel
	{
		public int ConversationId { get; set; }
		public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
		public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
	}
}
