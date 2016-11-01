using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Feedback;
using MobileSecondHand.API.Services.Mailing;
using MobileSecondHand.DB.Models.Feedback;
using MobileSecondHand.DB.Services.Feedback;

namespace MobileSecondHand.API.Services.Feedback
{
	public class FeedbackService : IFeedbackService
	{
		private IEmailService emailService;
		IFeedbackDbService feedbackDbService;

		public FeedbackService(IFeedbackDbService feedbackDbService, IEmailService emailService)
		{
			this.feedbackDbService = feedbackDbService;
			this.emailService = emailService;
		}

		public bool ReportWrongAdvertisement(string userId, WrongAdvertisementIssueModel model)
		{
			var issue = new WrongAdvertisementIssue();
			issue.IssueAuthorId = userId;
			issue.Reason = model.Reason;
			issue.AdvertisementId = model.AdvertisementId;

			feedbackDbService.SaveWrongAdvertisementIssue(issue);
			this.emailService.ReportWrongAdvertisementIssue(model);
			return true;
		}
	}
}
