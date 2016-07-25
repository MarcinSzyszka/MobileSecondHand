using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Models.Chat
{
	public class ConversationItemModel
	{
		public int Id { get; set; }
		public string InterLocutorId { get; set; }
		public string InterlocutorName { get; set; }
		public string LastMessage { get; set; }
		public string LastMessageDate { get; set; }
	}
}
