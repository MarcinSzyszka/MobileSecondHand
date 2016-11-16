using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Services.Factories;

namespace MobileSecondHand.Services
{
	public class CheckInternetConnectionService
	{
		private HttpClient client;

		public CheckInternetConnectionService(string bearerToken)
		{
			this.client = HttpClientFactory.GetHttpClientForCheckingConnection(bearerToken);
		}

		public async Task<bool> IsInternetConnectionActive()
		{
			try
			{
				var response = await this.client.GetAsync(WebApiConsts.WEB_API_ACCOUNT_CONTROLLER + "TokenIsActual");
			}
			catch (Exception)
			{
				//timeout exception after 5 sec
				return false;
			}

			return true;

		}
	}
}
