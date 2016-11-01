using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Feedback;
using MobileSecondHand.API.Services.Mailing.Config;
using Simplify.Mail;

namespace MobileSecondHand.API.Services.Mailing
{
	public class EmailService : IEmailService
	{
		MailSender mailSender;
		public EmailService(EmailSenderSettings settings)
		{
			mailSender = new MailSender(settings);
		}

		public void ReportWrongAdvertisementIssue(WrongAdvertisementIssueModel issueModel)
		{
			var mesageContent = String.Format("Ogłoszenie o id: {0} narusza regulamin. Powód: {1}", issueModel.AdvertisementId, issueModel.Reason);
			mailSender.Send("usersfeedback@mobilesecondhand.pl", "wrongadverts@mobilesecondhand.pl", "Ogłoszenie naruszające regulamin", mesageContent);
		}
	}
}
