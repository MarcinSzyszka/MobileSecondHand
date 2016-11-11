using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Chat;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Services.Factories;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Chat
{
	public class MessagesService : IMessagesService {
		private HttpClient client;

		public MessagesService(string bearerToken)
		{
			this.client = HttpClientFactory.GetHttpClient(bearerToken);
		}

		public async Task<List<ConversationMessage>> GetMessages(int conversationId, int pageNumber) {
			List<ConversationMessage> messagesList = new List<ConversationMessage>();

			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(String.Format("{0}/{1}/{2}/{3}", WebApiConsts.CONVERSATION_CONTROLLER, "GetMessages", conversationId, pageNumber));
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			
			if (response.IsSuccessStatusCode) {
				var responseString = await response.Content.ReadAsStringAsync();
				messagesList = JsonConvert.DeserializeObject<List<ConversationMessage>>(responseString);
			}

			return messagesList;
		}

		public async Task<ConversationInfoModel> GetConversationInfoModel(string addresseeId)
		{
			var conversationInfoModel = default(ConversationInfoModel);
			HttpResponseMessage response;
			try
			{
				 response = await client.GetAsync(String.Format("{0}/{1}/{2}", WebApiConsts.CONVERSATION_CONTROLLER, "GetConversationInfoModel", addresseeId));
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}

			if (response.IsSuccessStatusCode)
			{
				var responseString = await response.Content.ReadAsStringAsync();
				conversationInfoModel = JsonConvert.DeserializeObject<ConversationInfoModel>(responseString);
			}

			return conversationInfoModel;
		}

		public async Task<List<ConversationItemModel>> GetConversations(int pageNumber)
		{
			var conversations = new List<ConversationItemModel>();
			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(String.Format("{0}/{1}/{2}", WebApiConsts.CONVERSATION_CONTROLLER, "GetConversations", pageNumber));
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}

			if (response.IsSuccessStatusCode)
			{
				var responseString = await response.Content.ReadAsStringAsync();
				conversations = JsonConvert.DeserializeObject<List<ConversationItemModel>>(responseString);
			}

			return conversations;
		}
	}
}
