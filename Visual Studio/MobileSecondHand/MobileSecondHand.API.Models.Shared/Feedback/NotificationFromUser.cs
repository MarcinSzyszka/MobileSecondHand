using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Feedback
{
	public class NotificationFromUser
	{
		public string Title { get; set; }
		public string TelModel { get; set; }
		public string MessageContent { get; set; }
		public string UserId { get; set; }
		public string UserEmail { get; set; }
	}
}
