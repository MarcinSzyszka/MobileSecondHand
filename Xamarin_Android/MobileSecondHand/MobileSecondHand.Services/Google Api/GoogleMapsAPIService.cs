using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Google_Api
{
	public class GoogleMapsAPIService : IGoogleMapsAPIService
	{
		private string GetUrl(double lat, double lon)
		{
			return String.Format("https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&location_type=ROOFTOP&result_type=street_address&key=AIzaSyAw_LnL53KSsaUC1DZSUK9wgO72PBuXQQ4", lat.ToString().Replace(',', '.'), lon.ToString().Replace(',', '.'));

		}
		public async Task<string> GetAddress(double lat, double lon)
		{
			var resultAddress = String.Empty;
			var client = new HttpClient();
			//client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			//client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			var response = await client.GetAsync(GetUrl(lat, lon));
			var contentString = await response.Content.ReadAsStringAsync();

			var apiResults = JsonConvert.DeserializeObject<dynamic>(contentString);

			if (apiResults != null && apiResults.results[0] != null)
			{
				resultAddress = apiResults.results[0].formatted_address;
			}

			return resultAddress;
		}

	}
}
