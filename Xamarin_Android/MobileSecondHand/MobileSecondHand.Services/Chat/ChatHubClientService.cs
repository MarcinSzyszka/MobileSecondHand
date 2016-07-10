using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MobileSecondHand.Models.Consts;

namespace MobileSecondHand.Services.Chat {
	public class ChatHubClientService {
		IHubProxy chatHubProxy;
		HubConnection hubConnection;
		static ChatHubClientService serviceInstance;

		public ChatHubClientService(string bearerToken) {
			hubConnection = new HubConnection(WebApiConsts.SERWER_URL);
			chatHubProxy = hubConnection.CreateHubProxy("MobileSecondHandChatHub");
			hubConnection.Headers.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, bearerToken);
			Connect(hubConnection, async h => {
				await h.Start();
			});
		}

		public static ChatHubClientService GetServiceInstance(string bearerToken)
		{
			if (serviceInstance == null)
			{
				serviceInstance = new ChatHubClientService(bearerToken);
			}

			return serviceInstance;
		}

		public bool IsConnected()
		{
			return hubConnection.State == ConnectionState.Connected;
		}

		public void SendMessage(string messageContent, string addresseeId, int conversationId) {
			SendMessage(chatHubProxy, proxy => proxy.Invoke("SendMessage", messageContent, addresseeId, conversationId.ToString()));
		}

		public void RegisterReceiveMessages(Action<string, string, string, string> updateChatMessage) {
			chatHubProxy.On<string, string, string, string>("ReceiveMessage", (messageContent, messageHeader, conversationId, senderId) => updateChatMessage(messageContent, messageHeader, conversationId, senderId));
		}

		private void Connect(HubConnection hubObject, Action<HubConnection> hubConnection) {
			hubConnection(hubObject);
		}

		private void SendMessage(IHubProxy hubProxy, Action<IHubProxy> invoke) {
			invoke(hubProxy);
		}

		public static ChatHubClientService RecreateServiceInstance(string bearerToken)
		{
			
			serviceInstance = new ChatHubClientService(bearerToken);

			return serviceInstance;
		}

		public void Reconnect()
		{
				Connect(hubConnection, async h => {
				await h.Start();
			});
		}
	}
}
