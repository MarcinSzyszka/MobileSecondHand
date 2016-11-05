using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileSecondHand.API.Models.Shared.Feedback;
using MobileSecondHand.API.Services.Authentication;
using MobileSecondHand.API.Services.Feedback;
using MobileSecondHand.DB.Services.Feedback;

namespace MobileSecondHand.Controllers
{
	[Produces("application/json")]
	[Route("api/Feedback")]
	[Microsoft.AspNetCore.Authorization.Authorize("Bearer")]
	public class FeedbackController : Controller
	{
		IIdentityService identityService;
		private ILogger logger;
		IFeedbackService feedbackService;

		public FeedbackController(IIdentityService identityService, ILoggerFactory logger, IFeedbackService feedbackService)
		{
			this.identityService = identityService;
			this.logger = logger.CreateLogger<FeedbackController>();
			this.feedbackService = feedbackService;
		}

		[HttpPost]
		[Route("ReportWrongAdvertisement")]
		public IActionResult ReportWrongAdvertisement([FromBody] WrongAdvertisementIssueModel model)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				bool result = this.feedbackService.ReportWrongAdvertisement(userId, model);

				return Json("ok");
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wyst¹pi³ wyj¹tek w trakcie zg³aszania og³oszenia naruszaj¹cego regulamin: " + exc);
				return Json("Wyst¹pi³ b³¹d! - " + exc.Message);
			}
		}

		[HttpPost]
		[Route("SendNotificationFromUser")]
		public IActionResult SendNotificationFromUser([FromBody] NotificationFromUser model)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				bool result = this.feedbackService.SendNotificationFromUser(userId, User.Identity.Name, model);

				return Json("ok");
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wyst¹pi³ wyj¹tek w trakcie wysy³ania zg³oszenia od usera z formularza kontaktu: " + exc);
				return Json("Wyst¹pi³ b³¹d! - " + exc.Message);
			}
		}

		
	}
}