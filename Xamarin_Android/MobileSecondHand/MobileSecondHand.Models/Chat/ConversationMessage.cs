using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Models.Chat {
	public class ConversationMessage {
		public string SenderId { get; set; }
		public string MessageHeader { get; set; }
		public string MessageContent { get; set; }
		public bool UserWasSender { get; set; }
	}
}
