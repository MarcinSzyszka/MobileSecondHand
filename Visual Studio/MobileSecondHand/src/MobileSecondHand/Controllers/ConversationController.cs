using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileSecondHand.API.Services.Authentication;
using MobileSecondHand.API.Services.Conversation;

namespace MobileSecondHand.Controllers
{
	[Microsoft.AspNetCore.Authorization.Authorize("Bearer")]
	[Route("api/[controller]")]
	public class ConversationController : Controller
	{
		IConversationService conversationService;
		IIdentityService identityService;
		private ILogger logger;

		public ConversationController(IConversationService conversationService, IIdentityService identityService, ILoggerFactory loggerFactory)
		{
			this.conversationService = conversationService;
			this.identityService = identityService;
			this.logger = loggerFactory.CreateLogger<ConversationController>();
		}

		[HttpGet]
		[Route("GetMessages/{conversationId}/{pageNumber}")]
		public IActionResult GetMessages(int conversationId, int pageNumber)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var messages = this.conversationService.GetMessages(userId, conversationId, pageNumber);

				return Json(messages);
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wyst¹pi³ b³¹d podczas pobierania wiadomosci z rozmowy: " + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(String.Format("Wyst¹pi³ b³¹d: {0}", exc.Message));
			}
		}

		[HttpGet]
		[Route("GetConversationInfoModel/{addresseeId}")]
		public async Task<IActionResult> GetConversationInfoModel(string addresseeId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var conversationInfo = await this.conversationService.GetConversationInfoModel(userId, addresseeId);

				return Json(conversationInfo);
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wyst¹pi³ b³¹d podczas pobierania informacji o rozmowie: " + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(String.Format("Wyst¹pi³ b³¹d: {0}", exc.Message));
			}
		}

		[HttpGet]
		[Route("GetConversations/{pageNumber}")]
		public async Task<IActionResult> GetConversations(int pageNumber)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var conversations = await this.conversationService.GetConversations(userId, pageNumber);

				return Json(conversations);
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wyst¹pi³ b³¹d podczas pobierania rozmów: " + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(String.Format("Wyst¹pi³ b³¹d: {0}", exc.Message));
			}
		}


	}
}