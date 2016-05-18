using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MobileSecondHand.Models.Consts;

namespace MobileSecondHand.Services.Chat {
	public class ChatHubClientService {
		IHubProxy chatHubProxy;
		HubConnection hubConnection;

		public ChatHubClientService() {
			hubConnection = new HubConnection(WebApiConsts.SERWER_URL);
			chatHubProxy = hubConnection.CreateHubProxy("MobileSecondHandChatHub");
		}

		public void RegisterInHub(string bearerToken) {
			hubConnection.Headers.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, bearerToken);
			Connect(hubConnection, async h => {
				await h.Start();
				SendMessage(chatHubProxy, proxy => proxy.Invoke("Register", "Hello froom Xamarin Android App! :)"));
			});
		}

		public void RegisterReceiveMessages(Action<string> updateChatMessage) {
			chatHubProxy.On<string>("UpdateChatMessage", message => updateChatMessage(message));
		}

		private void Connect(HubConnection hubObject, Action<HubConnection> hubConnection) {
			hubConnection(hubObject);
		}

		private void SendMessage(IHubProxy hubProxy, Action<IHubProxy> invoke) {
			invoke(hubProxy);
		}
	}
}
