﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.Exceptions;
using MobileSecondHand.Services.Factories;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Authentication
{
	public class SignInService : ISignInService
	{
		private HttpClient client;

		public SignInService()
		{
			client = HttpClientFactory.GetHttpClient();
		}

		public async Task<bool> SignInUserWithBearerToken(TokenModel bearerToken)
		{
			if (client.DefaultRequestHeaders.Contains(WebApiConsts.AUTHORIZATION_HEADER_NAME))
			{
				client.DefaultRequestHeaders.Remove(WebApiConsts.AUTHORIZATION_HEADER_NAME);
			}
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken.Token);

			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "TokenIsActual");
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}

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

			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "SetUserName", stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}

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

			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "GetUserInfoModels/" + partName);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}

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

			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "UploadUserProfilePhoto", form);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
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


			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "GetUserProfileImage/" + interlocutorId);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
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
			HttpResponseMessage response = null;
			try
			{
				response = await client.PostAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + action, stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				if (response.StatusCode == System.Net.HttpStatusCode.NotImplemented)
				{
					return new TokenModel();
				}
				else
				{
					return null;
				}

			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var tokenModel = JsonConvert.DeserializeObject<TokenModel>(responseContentString);
			return tokenModel;
		}


	}
}
