using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Advertisement;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.DB.Models.Feedback
{
	public class WrongAdvertisementIssue
	{
		[Key]
		public int Id { get; set; }
		public string Reason { get; set; }
		public bool IsConsidered { get; set; }
		public int AdvertisementId { get; set; }
		[ForeignKey("AdvertisementId")]
		public AdvertisementItem Advertisement { get; set; }
		public string IssueAuthorId { get; set; }
		[ForeignKey("IssueAuthorId")]
		public ApplicationUser IssueAuthor { get; set; }
		public string ConsideredByUserId { get; set; }
		[ForeignKey("ConsideredByUserId")]
		public ApplicationUser ConsideredByUser { get; set; }
	}
}
