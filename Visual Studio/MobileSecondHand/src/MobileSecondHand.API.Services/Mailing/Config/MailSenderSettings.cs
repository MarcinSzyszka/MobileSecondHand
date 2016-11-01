using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplify.Mail;

namespace MobileSecondHand.API.Services.Mailing.Config
{
	public class EmailSenderSettings : IMailSenderSettings
	{
		public EmailSenderSettings(string userName, string userPassword, string serverAddress, int smtpPortNumber)
		{
			this.SmtpUserName = userName;
			this.SmtpUserPassword = userPassword;
			this.SmtpServerAddress = serverAddress;
			this.SmtpServerPortNumber = smtpPortNumber;
		}
		public int MyProperty { get; set; }
		public bool AntiSpamMessagesPoolOn
		{
			get { return false; }
		}

		public int AntiSpamPoolMessageLifeTime
		{
			get { return 100; }
		}

		public bool EnableSsl
		{
			get { return false; }
		}

		public string SmtpServerAddress
		{
			get;
			private set;
		}

		public int SmtpServerPortNumber
		{
			get;
			private set;
		}

		public string SmtpUserName
		{
			get;
			private set;
		}

		public string SmtpUserPassword
		{
			get;
			private set;
		}
	}
}
