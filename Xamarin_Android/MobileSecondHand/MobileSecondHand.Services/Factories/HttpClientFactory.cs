using System;
using System.Net.Http;
using MobileSecondHand.Models.Consts;

namespace MobileSecondHand.Services.Factories
{
	public class HttpClientFactory
	{
		static HttpClient client;
		public static HttpClient GetHttpClient(string bearerToken)
		{
			if (client == null)
			{
				client = new HttpClient();
				client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}
			else if (!client.DefaultRequestHeaders.Contains(WebApiConsts.AUTHORIZATION_HEADER_NAME))
			{
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}
			return client;
		}

		public static HttpClient GetHttpClient()
		{
			if (client == null)
			{
				client = new HttpClient();
				client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			}
			return client;
		}
	}
}
