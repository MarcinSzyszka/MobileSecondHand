using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.Security;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Authentication {
	public class SignInService : ISignInService {

		public async Task<bool> SignInUserWithBearerToken(TokenModel bearerToken) {
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken.Token);
			var response = await client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "TokenIsActual");

			return response.StatusCode == System.Net.HttpStatusCode.OK;
		}

		public async Task<TokenModel> SignInUserWithFacebookToken(FacebookTokenViewModel facebookToken) {
			return await GetTokenFromApi<FacebookTokenViewModel>(facebookToken, "LoginWithFacebook");
		}

		public async Task<TokenModel> SignInUserStandard(LoginModel loginModel) {
			return await GetTokenFromApi<LoginModel>(loginModel, "LoginStandard");
		}

		public async Task<TokenModel> RegisterUser(RegisterModel registerModel) {
			return await GetTokenFromApi<RegisterModel>(registerModel, "Register");
		}


		private async Task<TokenModel> GetTokenFromApi<T>(T modelToSend, string action) {
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			var stringContent = new StringContent(JsonConvert.SerializeObject(modelToSend), Encoding.UTF8, "application/json");

			var response = await client.PostAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + action, stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var tokenModel = JsonConvert.DeserializeObject<TokenModel>(responseContentString);
			return tokenModel;
		}
	}
}
