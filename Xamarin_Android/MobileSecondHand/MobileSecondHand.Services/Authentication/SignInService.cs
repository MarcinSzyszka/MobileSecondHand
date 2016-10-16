using System;
using System.Net.Http;
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
