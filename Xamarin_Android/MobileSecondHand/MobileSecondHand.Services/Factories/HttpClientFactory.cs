using System;
using System.Net.Http;
using MobileSecondHand.Models.Consts;
using ModernHttpClient;

namespace MobileSecondHand.Services.Factories
{
	public class HttpClientFactory
	{
		static HttpClient client;
		static HttpClient checkingConnectionClient;

		public static HttpClient GetHttpClient(string bearerToken)
		{
			if (client == null)
			{
				client = new HttpClient(new NativeMessageHandler());
				client.Timeout = new TimeSpan(0, 0, 0, 30, 0);
				client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}
			else if (!client.DefaultRequestHeaders.Contains(WebApiConsts.AUTHORIZATION_HEADER_NAME))
			{
				client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}
			return client;
		}

		public static HttpClient GetHttpClientForCheckingConnection(string bearerToken)
		{
			if (checkingConnectionClient == null)
			{
				checkingConnectionClient = new HttpClient(new NativeMessageHandler());
				checkingConnectionClient.Timeout = new TimeSpan(0, 0, 0, 5, 0);
				checkingConnectionClient.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
				checkingConnectionClient.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
			}

			return checkingConnectionClient;
		}

		public static HttpClient GetHttpClient()
		{
			if (client == null)
			{
				client = new HttpClient(new NativeMessageHandler());
				client.Timeout = new TimeSpan(0, 0, 0, 30, 0);
				client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			}
			return client;
		}
	}
}
