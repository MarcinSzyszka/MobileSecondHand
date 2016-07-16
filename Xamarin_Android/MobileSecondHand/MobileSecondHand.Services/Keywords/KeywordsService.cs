using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Consts;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Keywords
{
	public class KeywordsService : IKeyworsService
	{
		public async Task<IDictionary<int, string>> GetKeywords(string bearerToken)
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			var response = await client.GetAsync(WebApiConsts.KEYWORDS_CONTROLLER + "GetKeywordsForSettings");
			var contentString = await response.Content.ReadAsStringAsync();

			var keywords = JsonConvert.DeserializeObject<IDictionary<int, string>>(contentString);

			return keywords;
		}
	}
}
