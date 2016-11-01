using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Feedback;
using MobileSecondHand.Models.Consts;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Feedback
{
	public class FeedbackService : IFeedbackService
	{
		private HttpClient client;

		public FeedbackService(string bearerToken)
		{
			this.client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
		}

		public async Task<bool> ReportWrongAdvertisement(int advertId, string reason)
		{
			var model = new WrongAdvertisementIssueModel();
			model.Reason = reason;
			model.AdvertisementId = advertId;
			var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

			var response = await client.PostAsync(WebApiConsts.FEEDBACK_CONTROLLER + "ReportWrongAdvertisement/", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}

			return true;
		}
	}
}
