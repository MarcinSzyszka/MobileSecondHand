using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using MobileSecondHand.Api.Models.OutsideApisModels;
using MobileSecondHand.Api.Models.Security;
using MobileSecondHand.Api.Services.OutsideApisManagers;
using MobileSecondHand.Db.Models;

namespace MobileSecondHand.Api.Services.Authentication {
	public class ApplicationSignInManager : IApplicationSignInManager {
		SignInManager<ApplicationUser> signInManager;
		IApplicationUserManager applicationUserManager;
		IFacebookApiManager facebookApiManager;
		TokenAuthorizationOptions tokenAuthorizationOptions;

		public ApplicationSignInManager(SignInManager<ApplicationUser> signInManager, IApplicationUserManager applicationUserManager, IFacebookApiManager facebookApiManager, TokenAuthorizationOptions tokenAuthorizationOptions) {
			this.signInManager = signInManager;
			this.applicationUserManager = applicationUserManager;
			this.facebookApiManager = facebookApiManager;
			this.tokenAuthorizationOptions = tokenAuthorizationOptions;
		}

		public async Task<TokenModel> Register(RegisterViewModel registerViewModel) {
			if (registerViewModel.Email == null || registerViewModel.Password == null) {
				throw new Exception("Register model is invalid");
			}
			ApplicationUser user = await CreateUser(registerViewModel);
			return GetToken(user, DateTime.UtcNow.AddMinutes(30));
		}

		public async Task<TokenModel> LoginStandard(LoginViewModel loginStandardViewModel) {
			if (loginStandardViewModel.Email == null || loginStandardViewModel.Password == null) {
				throw new Exception("Login model is invalid");
			}
			ApplicationUser user = await applicationUserManager.GetUser(loginStandardViewModel.Email);
			if (user == null) {
				throw new Exception("Użytkownik o podanym adresie email nieistnieje");
			}
			var passwordIsValid = await applicationUserManager.PasswordIsValid(user, loginStandardViewModel.Password);
			if (!passwordIsValid) {
				throw new Exception("Hasło jest nieprawidłowe");
			}
			return GetToken(user, DateTime.UtcNow.AddMinutes(30));
		}

		public async Task<TokenModel> LoginWithFacebook(string facebookToken) {
			FacebookUserCredentialsResponse facebookResponse = await facebookApiManager.GetUserCredentials(facebookToken);

			if (facebookResponse.email != null) {
				ApplicationUser user = await applicationUserManager.GetUser(facebookResponse.email);
				if (user == null) {
					user = await CreateUser(facebookResponse);
				}
				return GetToken(user, DateTime.UtcNow.AddMinutes(30));
			}
			throw new Exception("Facebook not returned email address");
		}

		private async Task<ApplicationUser> CreateUser(FacebookUserCredentialsResponse facebookResponse) {
			ApplicationUser user = new ApplicationUser { UserName = facebookResponse.email, Email = facebookResponse.email };
			IdentityResult result = await applicationUserManager.CreateAsync(user);
			if (result.Succeeded) {
				IdentityResult loginInfoResult = await applicationUserManager.AddLoginAsync(user, new UserLoginInfo("Facebook", facebookResponse.id, facebookResponse.name));
				if (!loginInfoResult.Succeeded) {
					throw new Exception("Creating new user failed");
				}
			}
			else {
				throw new Exception("Creating new user failed");
			}

			return user;
		}
		private async Task<ApplicationUser> CreateUser(RegisterViewModel registerViewModel) {
			ApplicationUser user = new ApplicationUser { UserName = registerViewModel.Email, Email = registerViewModel.Email };
			IdentityResult result = await applicationUserManager.CreateAsync(user, registerViewModel.Password);
			if (!result.Succeeded) {
				throw new Exception("Creating new user failed");
			}

			return user;
		}
		private TokenModel GetToken(ApplicationUser user, DateTime? expires) {
			var handler = new JwtSecurityTokenHandler();

			ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user.Email, "TokenAuth"), new[] { new Claim("UserId", user.Id, ClaimValueTypes.String) });

			var securityToken = handler.CreateToken(
				issuer: tokenAuthorizationOptions.Issuer,
				audience: tokenAuthorizationOptions.Audience,
				signingCredentials: tokenAuthorizationOptions.SigningCredentials,
				subject: identity,
				expires: expires
				);

			return new TokenModel { Token = handler.WriteToken(securityToken) };
		}
	}
}
