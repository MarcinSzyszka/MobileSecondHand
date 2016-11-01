using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Feedback;
using MobileSecondHand.DB.Models.Feedback;
using MobileSecondHand.DB.Services.Feedback;

namespace MobileSecondHand.API.Services.Feedback
{
	public class FeedbackService : IFeedbackService
	{
		IFeedbackDbService feedbackDbService;

		public FeedbackService(IFeedbackDbService feedbackDbService)
		{
			this.feedbackDbService = feedbackDbService;
		}

		public bool ReportWrongAdvertisement(string userId, WrongAdvertisementIssueModel model)
		{
			var issue = new WrongAdvertisementIssue();
			issue.IssueAuthorId = userId;
			issue.Reason = model.Reason;
			issue.AdvertisementId = model.AdvertisementId;

			feedbackDbService.SaveWrongAdvertisementIssue(issue);

			return true;
		}
	}
}
