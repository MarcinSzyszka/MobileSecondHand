﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Models.Chat {
	public class ConversationMessage {
		public int Id { get; set; }
		/// <summary>
		/// Author name and datetime
		/// </summary>
		public string MessageHeader { get; set; }
		public string MessageContent { get; set; }
		/// <summary>
		/// Value indicates if user who is downloading this message was author of it
		/// </summary>
		public bool UserWasSender { get; set; }
		public int ConversationId { get; set; }
		public string SenderId { get; set; }
		public string SenderName { get; set; }
	}
}
