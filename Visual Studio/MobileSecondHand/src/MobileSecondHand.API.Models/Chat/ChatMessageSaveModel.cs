using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Chat
{
	public class ChatMessageSaveModel
	{
		public int ConversationId { get; set; }
		public string SenderId { get; set; }
		public string AddresseeId { get; set; }
		public string Content { get; set; }
		public bool AddresseeCanReceiveMessage { get; set; }
	}
}
