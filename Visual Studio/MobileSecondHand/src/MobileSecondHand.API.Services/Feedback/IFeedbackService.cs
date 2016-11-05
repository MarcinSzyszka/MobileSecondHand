using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Feedback;

namespace MobileSecondHand.API.Services.Feedback
{
	public interface IFeedbackService
	{
		bool ReportWrongAdvertisement(string userId, WrongAdvertisementIssueModel model);
		bool SendNotificationFromUser(string userId, string userEmail, NotificationFromUser model);
	}
}
