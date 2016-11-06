﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Consts;
using ModernHttpClient;

namespace MobileSecondHand.Services.Factories
{
	public class HttpClientFactory
	{
		static HttpClient client;
		public static HttpClient GetHttpClient(string bearerToken)
		{
			if (client == null)
			{
				client = new HttpClient(new NativeMessageHandler());
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
				client = new HttpClient(new NativeMessageHandler());
				client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			}
			return client;
		}
	}
}
