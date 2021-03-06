﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Chat
{
	public class ConversationInfoModel
	{
		public int ConversationId { get; set; }
		public string InterlocutorId { get; set; }
		public string InterlocutorName { get; set; }
		public byte[] InterlocutorPrifileImage { get; set; }
	}
}
