using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.Security;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Advertisements {
	public class AdvertisementItemService : IAdvertisementItemService {
		public async Task<List<AdvertisementItemShort>> GetAdvertisements(SearchModel searchModel, TokenModel tokenModel) {
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + tokenModel.Token);
			var stringContent = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisements", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}
	}
}
