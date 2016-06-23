using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.DB.Models.Chat {
	public class UserToConversation {
		public int ConversationId { get; set; }
		public Conversation Conversation { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
	}
}
