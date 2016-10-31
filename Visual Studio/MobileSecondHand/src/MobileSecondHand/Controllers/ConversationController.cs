using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

		public ConversationController(IConversationService conversationService, IIdentityService identityService)
		{
			this.conversationService = conversationService;
			this.identityService = identityService;
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
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(String.Format("Wyst¹pi³ b³¹d: {0}", exc.Message));
			}
		}

		[HttpGet]
		[Route("GetConversationInfoModel/{addresseeId}")]
		public IActionResult GetConversationInfoModel(string addresseeId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var conversationInfo = this.conversationService.GetConversationInfoModel(userId, addresseeId);

				return Json(conversationInfo);
			}
			catch (Exception exc)
			{
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
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(String.Format("Wyst¹pi³ b³¹d: {0}", exc.Message));
			}
		}


	}
}