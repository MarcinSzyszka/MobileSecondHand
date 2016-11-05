using System;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Services.Authentication;
using System.Net;
using MobileSecondHand.API.Models.CustomResponsesModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MobileSecondHand.API.Models.Shared.Security;
using Microsoft.Extensions.Logging;

namespace MobileSecondHand.Controllers
{
	[Route("api/[controller]")]
	public class WebApiAccountController : Controller
	{
		IApplicationSignInManager applicationSignInManager;
		IIdentityService identityService;
		private ILogger logger;

		public WebApiAccountController(IApplicationSignInManager applicationSignInManager, IIdentityService identityService, ILoggerFactory loggerFactory)
		{
			this.applicationSignInManager = applicationSignInManager;
			this.identityService = identityService;
			this.logger = loggerFactory.CreateLogger<WebApiAccountController>(); ;
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
				this.logger.LogError("Wystąpił błąd podczas logowania z facebookiem:" + exc);
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
				this.logger.LogError("Wystąpił błąd podczas logowania standardowego" + exc);
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
				this.logger.LogError("Wystąpił błąd podczas standradowej rejestracji" + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Authorize("Bearer")]
		[Route("SetUserName")]
		public async Task<IActionResult> SetUserName([FromBody]string nickName)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var nameIsSet = await this.applicationSignInManager.SetUserName(userId, nickName);
				if (!nameIsSet)
				{
					Response.StatusCode = (int)HttpStatusCode.Conflict;
				}
				return Json("ok");
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wystąpił błąd podczas ustawaiania nick'u prezez usera" + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpGet]
		[Authorize("Bearer")]
		[Route("GetUserInfoModels/{partName}")]
		public IActionResult GetUserInfoModels(string partName)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var userNamesModels = this.applicationSignInManager.GetUserNamesModels(userId, partName);

				return Json(userNamesModels);
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wystąpił błąd podczas pobierania nazw i idików userów" + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost("UploadUserProfilePhoto")]
		[Authorize("Bearer")]
		public async Task<IActionResult> UploadUserProfilePhoto()
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var result = await this.applicationSignInManager.SaveUserProfilePhoto(userId, Request.Form.Files);

				return Json("ok");
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wystąpił błąd podczas uploadu profilowego zdjęcia usera :" + exc);
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
				this.logger.LogError("Wystąpił błąd podczas sprawdzania waznosci tokena bearer" + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}


		[HttpGet]
		[Authorize("Bearer")]
		[Route("GetUserProfileImage/{requstedUserId}")]
		public async Task<IActionResult> GetUserProfileImage(string requstedUserId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				byte[] profileImageBytes = await this.applicationSignInManager.GetUserProfileImage(requstedUserId);

				return Json(profileImageBytes);
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wystąpił błąd podczas pobierania profilowego zdjęcia usera" + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return null;
			}
		}


	}
}
