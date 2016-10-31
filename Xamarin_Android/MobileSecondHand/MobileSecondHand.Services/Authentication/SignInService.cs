using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.Exceptions;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Authentication
{
	public class SignInService : ISignInService
	{
		private HttpClient client;

		public SignInService()
		{
			client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
		}

		public async Task<bool> SignInUserWithBearerToken(TokenModel bearerToken)
		{
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken.Token);
			var response = await client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "TokenIsActual");

			if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
			{
				throw new UserHasToSetNickNameException();
			}
			return response.StatusCode == System.Net.HttpStatusCode.OK;
		}

		public async Task<TokenModel> SignInUserWithFacebookToken(FacebookTokenViewModel facebookToken)
		{
			return await GetTokenFromApi<FacebookTokenViewModel>(facebookToken, "LoginWithFacebook");
		}

		public async Task<TokenModel> SignInUserStandard(LoginModel loginModel)
		{
			return await GetTokenFromApi<LoginModel>(loginModel, "LoginStandard");
		}

		public async Task<TokenModel> RegisterUser(RegisterModel registerModel)
		{
			return await GetTokenFromApi<RegisterModel>(registerModel, "Register");
		}

		public async Task<bool> SetUserName(string nickName, TokenModel bearerToken)
		{
			if (!client.DefaultRequestHeaders.Contains(WebApiConsts.AUTHORIZATION_HEADER_NAME))
			{
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken.Token);
			}

			var stringContent = new StringContent(JsonConvert.SerializeObject(nickName), Encoding.UTF8, "application/json");


			var response = await client.PostAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "SetUserName", stringContent);

			if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
			{
				return false;
			}
			else if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new Exception("Wystąpił błą połączenia z serwerem");
			}
			else
			{
				return true;
			}


		}

		public async Task<IEnumerable<UserInfoModel>> GetUserNamesModels(string bearerToken, string partName)
		{
			if (!client.DefaultRequestHeaders.Contains(WebApiConsts.AUTHORIZATION_HEADER_NAME))
			{
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}

			var response = await client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "GetUserInfoModels/" + partName);

			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return new List<UserInfoModel>();
			}

			var modelsString = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<IEnumerable<UserInfoModel>>(modelsString);
		}

		public async Task<bool> UploadUserProfilePhoto(string bearerToken, byte[] photoByteArray)
		{
			if (!client.DefaultRequestHeaders.Contains(WebApiConsts.AUTHORIZATION_HEADER_NAME))
			{
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}

			MultipartFormDataContent form = new MultipartFormDataContent();
			HttpContent content = new StringContent("uploadPhoto");
			var stream = new MemoryStream(photoByteArray);
			content = new StreamContent(stream);
			content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = "profilePhoto",
				FileName = "profilePhoto"
			};
			form.Add(content);

			var response = await client.PostAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "UploadUserProfilePhoto", form);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}

			return true;
		}

		public async Task<byte[]> GetUserProfileImage(string bearerToken, string interlocutorId)
		{
			if (!client.DefaultRequestHeaders.Contains(WebApiConsts.AUTHORIZATION_HEADER_NAME))
			{
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}

			var response = await client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "GetUserProfileImage/" + interlocutorId);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return null;
			}
			byte[] imageBytes;
			try
			{
				var arrayString = await response.Content.ReadAsStringAsync();
				imageBytes = JsonConvert.DeserializeObject<byte[]>(arrayString);
			}
			catch (Exception)
			{
				imageBytes = null;
			}


			return imageBytes;
		}

		private async Task<TokenModel> GetTokenFromApi<T>(T modelToSend, string action)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(modelToSend), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + action, stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var tokenModel = JsonConvert.DeserializeObject<TokenModel>(responseContentString);
			return tokenModel;
		}


	}
}
