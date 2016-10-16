using System;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Services.Authentication;
using System.Net;
using MobileSecondHand.API.Models.CustomResponsesModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.Controllers
{
	[Route("api/[controller]")]
	public class WebApiAccountController : Controller
	{
		IApplicationSignInManager applicationSignInManager;
		IIdentityService identityService;

		public WebApiAccountController(IApplicationSignInManager applicationSignInManager, IIdentityService identityService)
		{
			this.applicationSignInManager = applicationSignInManager;
			this.identityService = identityService;
		}

		[HttpPost]
		[Route("LoginWithFacebook")]
		public async Task<IActionResult> LoginWithFacebook([FromBody]FacebookTokenViewModel facebookToken)
		{
			try
			{
				var token = await this.applicationSignInManager.LoginWithFacebook(facebookToken);

				return Json(token);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("LoginStandard")]
		public async Task<IActionResult> LoginStandard([FromBody]LoginModel loginViewModel)
		{
			try
			{
				var token = await this.applicationSignInManager.LoginStandard(loginViewModel);
				return Json(token);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody]RegisterModel registerViewModel)
		{
			try
			{
				var token = await this.applicationSignInManager.Register(registerViewModel);
				return Json(token);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}


		[HttpGet]
		[Authorize("Bearer")]
		[Route("TokenIsActual")]
		public async Task<IActionResult> TokenIsActual()
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var userNameIsSet = await this.applicationSignInManager.IsUserNameSetByHimself(userId);
				if (!userNameIsSet)
				{
					//by this statuscode i will know that user have to set their nick name
					Response.StatusCode = (int)HttpStatusCode.NotModified;
				}

				return Json("Ok");
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}
	}
}
