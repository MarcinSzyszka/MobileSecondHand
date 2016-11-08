using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Feedback;
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

		public void SendNotificationFromSite(EmailFromSiteModel model)
		{
			var subject = "Nowa wiadomość ze strony internetowej.";
			var mesageContent = String.Format("{0} <br/> Nazwa nadawcy: {1} <br/> Email nadawcy: {2}", model.Message, model.Name, model.Email);
			mailSender.Send(model.Email, "admin@mobilesecondhand.pl", subject, mesageContent);
		}

		public void SendNotificationFromUser(NotificationFromUser model)
		{
			var subject = String.Format("Nowa wiadomość od użytkownika: {0}", model.Title);
			var mesageContent = String.Format("{0} <br/> Id użytkownika: {1} <br/> Email użytkownika: {2}", model.MessageContent, model.UserId, model.UserEmail);
			mailSender.Send(model.UserEmail, "admin@mobilesecondhand.pl", subject, mesageContent);
		}
	}
}
