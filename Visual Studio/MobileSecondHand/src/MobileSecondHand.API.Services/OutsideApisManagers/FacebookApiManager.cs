using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MobileSecondHand.API.Models.OutsideApisModels;

namespace MobileSecondHand.API.Services.OutsideApisManagers {
	public class FacebookApiManager : IFacebookApiManager {
		const string graphUrl = @"https://graph.facebook.com/me?access_token=";
		public async Task<FacebookUserCredentialsResponse> GetUserCredentials(string facebookToken) {
			var url = String.Format("{0}{1}&fields=email,name", graphUrl, facebookToken);
			HttpClient client = new HttpClient();
			var response = await client.GetAsync(url);
			var respText = await response.Content.ReadAsStringAsync();
			FacebookUserCredentialsResponse facebookResponse = new JavaScriptSerializer().Deserialize<FacebookUserCredentialsResponse>(respText);

			return facebookResponse;
		}
	}
}
