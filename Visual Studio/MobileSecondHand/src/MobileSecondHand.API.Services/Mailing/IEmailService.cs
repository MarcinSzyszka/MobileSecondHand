﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Feedback;
using MobileSecondHand.API.Models.Shared.Feedback;

namespace MobileSecondHand.API.Services.Mailing
{
	public interface IEmailService
	{
		void ReportWrongAdvertisementIssue(WrongAdvertisementIssueModel issueModel);
		void SendNotificationFromUser(NotificationFromUser model);
		void SendNotificationFromSite(EmailFromSiteModel model);
	}
}
