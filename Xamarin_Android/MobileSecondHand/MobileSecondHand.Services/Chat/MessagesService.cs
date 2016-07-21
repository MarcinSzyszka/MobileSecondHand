using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.Security;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Chat {
	public class MessagesService : IMessagesService {

		public async Task<List<ConversationMessage>> GetMessages(int conversationId, int pageNumber, string bearerToken) {
			List<ConversationMessage> messagesList = new List<ConversationMessage>();
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);

			var response = await client.GetAsync(String.Format("{0}/{1}/{2}/{3}", WebApiConsts.CONVERSATION_CONTROLLER, "GetMessages", conversationId, pageNumber));
			
			if (response.IsSuccessStatusCode) {
				var responseString = await response.Content.ReadAsStringAsync();
				messagesList = JsonConvert.DeserializeObject<List<ConversationMessage>>(responseString);
			}

			return messagesList;
		}

		public async Task<ConversationInfoModel> GetConversationInfoModel(string addresseeId, string bearerToken)
		{
			var conversationInfoModel = default(ConversationInfoModel);
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);

			var response = await client.GetAsync(String.Format("{0}/{1}/{2}", WebApiConsts.CONVERSATION_CONTROLLER, "GetConversationInfoModel", addresseeId));

			if (response.IsSuccessStatusCode)
			{
				var responseString = await response.Content.ReadAsStringAsync();
				conversationInfoModel = JsonConvert.DeserializeObject<ConversationInfoModel>(responseString);
			}

			return conversationInfoModel;
		}
	}
}
