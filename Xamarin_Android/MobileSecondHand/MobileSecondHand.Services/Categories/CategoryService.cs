using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Services.Factories;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Categories
{
	public class CategoryService : ICategoryService

	{
		private HttpClient client;

		public CategoryService(string bearerToken)
		{
			this.client = HttpClientFactory.GetHttpClient(bearerToken);
		}

		public async Task<IDictionary<int, string>> GetCategories()
		{
			var response = await client.GetAsync(WebApiConsts.CATEGORY_CONTROLLER + "GetCategoriesForSettings");
			var contentString = await response.Content.ReadAsStringAsync();

			var keywords = JsonConvert.DeserializeObject<IDictionary<int, string>>(contentString);

			return keywords;
		}
	}
}
