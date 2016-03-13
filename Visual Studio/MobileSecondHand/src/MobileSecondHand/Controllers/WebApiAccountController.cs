﻿using Microsoft.AspNet.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using MobileSecondHand.Api.Models.Security;
using MobileSecondHand.Api.Services.Authentication;
using System.Net;
using MobileSecondHand.Api.Models.CustomResponsesModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MobileSecondHand.Api.Models.Advertisement;
using System.IO;

namespace MobileSecondHand.Controllers {
	[Route("api/[controller]")]
	public class WebApiAccountController : Controller {
		IApplicationSignInManager applicationSignInManager;

		public WebApiAccountController(IApplicationSignInManager applicationSignInManager) {
			this.applicationSignInManager = applicationSignInManager;
		}

		[HttpPost]
		[Route("LoginWithFacebook")]
		public async Task<IActionResult> LoginWithFacebook(string facebookToken) {
			try {
				var token = await this.applicationSignInManager.LoginWithFacebook(facebookToken);
				return Json(token);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("LoginStandard")]
		public async Task<IActionResult> LoginStandard([FromBody]LoginViewModel loginViewModel) {
			try {
				var token = await this.applicationSignInManager.LoginStandard(loginViewModel);
				return Json(token);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody]RegisterViewModel registerViewModel) {
			try {
				var token = await this.applicationSignInManager.Register(registerViewModel);
				return Json(token);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpGet]
		public string GetToken() {
			//return GetToken("admin", DateTime.Now.AddMinutes(30));
			return null;
		}


		[HttpGet]
		[Authorize("Bearer")]
		[Route("TokenIsActual")]
		public JsonResult TokenIsActual() {
			//var handler = new JwtSecurityTokenHandler();
			//if (User.Identity.IsAuthenticated) {
			//	string tokenHeader = Request.Headers["Authorization"];
			//	string stringToken = tokenHeader.Substring(7);
			//	var token = handler.ReadJwtToken(stringToken);
			//	var time = handler.TokenLifetimeInMinutes;
			//	handler.TokenLifetimeInMinutes = 100;
			//	var time2 = handler.TokenLifetimeInMinutes;
			//	var claims = (ClaimsIdentity)User.Identity;
			//}
			return Json("Ok");
		}


	}
}