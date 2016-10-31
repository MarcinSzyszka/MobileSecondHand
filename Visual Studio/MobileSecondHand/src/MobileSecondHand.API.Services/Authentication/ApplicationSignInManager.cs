using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MobileSecondHand.API.Models.OutsideApisModels;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.API.Services.OutsideApisManagers;
using MobileSecondHand.API.Services.Photos;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.API.Services.Authentication
{
	public class ApplicationSignInManager : IApplicationSignInManager
	{
		SignInManager<ApplicationUser> signInManager;
		IApplicationUserManager applicationUserManager;
		IFacebookApiManager facebookApiManager;
		IPhotosService advertisementItemPhotosUploader;
		TokenAuthorizationOptions tokenAuthorizationOptions;

		public ApplicationSignInManager(SignInManager<ApplicationUser> signInManager, IApplicationUserManager applicationUserManager, IFacebookApiManager facebookApiManager, IPhotosService advertisementItemPhotosUploader, TokenAuthorizationOptions tokenAuthorizationOptions)
		{
			this.signInManager = signInManager;
			this.applicationUserManager = applicationUserManager;
			this.facebookApiManager = facebookApiManager;
			this.advertisementItemPhotosUploader = advertisementItemPhotosUploader;
			this.tokenAuthorizationOptions = tokenAuthorizationOptions;
		}

		public async Task<TokenModel> Register(RegisterModel registerViewModel)
		{
			if (registerViewModel.Email == null || registerViewModel.Password == null)
			{
				throw new Exception("Register model is invalid");
			}
			ApplicationUser user = await CreateUser(registerViewModel);
			return GetToken(user);
		}

		public async Task<TokenModel> LoginStandard(LoginModel loginStandardViewModel)
		{
			if (loginStandardViewModel.Email == null || loginStandardViewModel.Password == null)
			{
				throw new Exception("Login model is invalid");
			}
			ApplicationUser user = await applicationUserManager.GetUserByEmail(loginStandardViewModel.Email);
			if (user == null)
			{
				//	throw new Exception("Użytkownik o podanym adresie email nieistnieje");
			}
			var passwordIsValid = await applicationUserManager.PasswordIsValid(user, loginStandardViewModel.Password);
			if (!passwordIsValid)
			{
				throw new Exception("Hasło jest nieprawidłowe");
			}
			return GetToken(user);
		}

		public async Task<TokenModel> LoginWithFacebook(FacebookTokenViewModel facebookToken)
		{
			FacebookUserCredentialsResponse facebookResponse = await facebookApiManager.GetUserCredentials(facebookToken.FacebookToken);

			if (facebookResponse.email != null)
			{
				ApplicationUser user = await applicationUserManager.GetUserByEmail(facebookResponse.email);
				if (user == null)
				{
					user = await CreateUser(facebookResponse);
				}
				return GetToken(user);
			}
			throw new Exception("Facebook not returned email address");
		}

		public async Task<bool> IsUserNameSetByHimself(string userId)
		{
			ApplicationUser user = await applicationUserManager.GetUserById(userId);

			return user.UserNameIsSetByHimself;
		}

		public async Task<bool> SetUserName(string userId, string nickName)
		{
			var user = await applicationUserManager.GetByUserName(nickName);

			if (user != null)
			{
				return false;
			}

			user = await applicationUserManager.GetUserById(userId);
			user.UserNameIsSetByHimself = true;

			var result = await applicationUserManager.SaveUserName(user, nickName);


			return result.Succeeded;
		}

		public IEnumerable<UserInfoModel> GetUserNamesModels(string userId, string partName)
		{
			var resultList = new List<UserInfoModel>();
			var users = applicationUserManager.GetAllUsers().Where(u => u.UserName.ToLower().StartsWith(partName.ToLower())).ToList();

			foreach (var user in users)
			{
				resultList.Add(new UserInfoModel { Id = user.Id, UserName = user.UserName });
			}

			return resultList;
		}

		public async Task<bool> SaveUserProfilePhoto(string userId, IFormFileCollection files)
		{
			var user = await this.applicationUserManager.GetUserById(userId);
			if (user == null)
			{
				throw new Exception("User o podanym id nie istnieje");
			}
			var photoName = this.advertisementItemPhotosUploader.SaveUserProfilePhoto(files);
			user.UserProfilePhotoName = photoName;
			var result = await this.applicationUserManager.UpdateUserMdel(user);
			if (!result.Succeeded)
			{
				throw new Exception("Wystąpił błąd podczas update'u usera w bazie");
			}

			return true;
		}

		public async Task<byte[]> GetUserProfileImage(string requstedUserId)
		{
			var user = await this.applicationUserManager.GetUserById(requstedUserId);
			if (user == null)
			{
				throw new Exception("User o podanym id nie istnieje");
			}

			return await this.advertisementItemPhotosUploader.GetUserProfilePhotoInBytes(user.UserProfilePhotoName);
		}

		private async Task<ApplicationUser> CreateUser(FacebookUserCredentialsResponse facebookResponse)
		{
			ApplicationUser user = new ApplicationUser { UserName = facebookResponse.email, Email = facebookResponse.email };
			IdentityResult result = await applicationUserManager.CreateAsync(user);
			if (result.Succeeded)
			{
				IdentityResult loginInfoResult = await applicationUserManager.AddLoginAsync(user, new UserLoginInfo("Facebook", facebookResponse.id, facebookResponse.name));
				if (!loginInfoResult.Succeeded)
				{
					throw new Exception("Creating new user failed");
				}
			}
			else
			{
				throw new Exception("Creating new user failed");
			}

			return user;
		}
		private async Task<ApplicationUser> CreateUser(RegisterModel registerViewModel)
		{
			ApplicationUser user = new ApplicationUser { UserName = registerViewModel.Email, Email = registerViewModel.Email };
			IdentityResult result = await applicationUserManager.CreateAsync(user, registerViewModel.Password);
			if (!result.Succeeded)
			{
				throw new Exception("Creating new user failed");
			}

			return user;
		}
		private TokenModel GetToken(ApplicationUser user)
		{
			var handler = new JwtSecurityTokenHandler();

			ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user.Email, "TokenAuth"), new[] { new Claim("UserId", user.Id, ClaimValueTypes.String) });

			var securityToken = handler.CreateToken(GetTokenOptions(identity));

			return new TokenModel { Token = handler.WriteToken(securityToken), UserHasToSetNickName = !user.UserNameIsSetByHimself, UserName = user.UserName };
		}

		private SecurityTokenDescriptor GetTokenOptions(ClaimsIdentity identity)
		{
			var tokenDescriptor = new SecurityTokenDescriptor();
			tokenDescriptor.Issuer = tokenAuthorizationOptions.Issuer;
			tokenDescriptor.Audience = tokenAuthorizationOptions.Audience;
			tokenDescriptor.SigningCredentials = tokenAuthorizationOptions.SigningCredentials;
			tokenDescriptor.Subject = identity;
			tokenDescriptor.Expires = DateTime.Now.AddDays(24);

			return tokenDescriptor;
		}


	}
}
