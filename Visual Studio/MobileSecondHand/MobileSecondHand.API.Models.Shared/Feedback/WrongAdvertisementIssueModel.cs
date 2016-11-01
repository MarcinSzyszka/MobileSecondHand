using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Feedback
{
	public class WrongAdvertisementIssueModel
	{
		public string Reason { get; set; }
		public int AdvertisementId { get; set; }
	}
}
