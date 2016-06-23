using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.DB.Models.Chat {
	public class ChatMessage {
		public int ChatMessageId { get; set; }
		public DateTime Date { get; set; }
		public string Content { get; set; }

		public string AuthorId { get; set; }
		[ForeignKey("AuthorId")]
		public ApplicationUser Author { get; set; }

		public int ConversationId { get; set; }
		[ForeignKey("ConversationId")]
		public Conversation Conversation { get; set; }
	}
}
