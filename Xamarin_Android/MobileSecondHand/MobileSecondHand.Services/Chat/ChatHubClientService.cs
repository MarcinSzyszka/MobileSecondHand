using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MobileSecondHand.Models.Consts;

namespace MobileSecondHand.Services.Chat
{
	public class ChatHubClientService : IDisposable
	{
		IHubProxy chatHubProxy;
		HubConnection hubConnection;
		static ChatHubClientService serviceInstance;

		public ChatHubClientService(string bearerToken)
		{
			hubConnection = new HubConnection(WebApiConsts.SERWER_URL);
			chatHubProxy = hubConnection.CreateHubProxy("MobileSecondHandChatHub");
			hubConnection.Headers.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, bearerToken);
			Connect(hubConnection, async h =>
			{
				await h.Start();
			});
		}

		public static ChatHubClientService GetServiceInstance(string bearerToken)
		{
			if (serviceInstance == null)
			{
				serviceInstance = new ChatHubClientService(bearerToken);
			}
			else if (!serviceInstance.IsConnected())
			{
				serviceInstance.Reconnect();
			}

			return serviceInstance;
		}

		public bool IsConnected()
		{
			return hubConnection.State == ConnectionState.Connected;
		}

		public void SendMessage(string messageContent, string addresseeId, int conversationId)
		{
			SendMessage(chatHubProxy, proxy => proxy.Invoke("SendMessage", messageContent, addresseeId, conversationId.ToString()));
		}

		public void MessageReceived(int messageId)
		{
			MessageReceived(chatHubProxy, proxy => proxy.Invoke("MessageReceived", messageId.ToString()));
		}


		public void RegisterReceiveMessages(Action<string> updateChatMessage)
		{
			chatHubProxy.On<string>("ReceiveMessage", updateChatMessage);
		}

		public void RegisterNotReceivedMessagesChecked(Action setCheckedNotReceivedMessages)
		{
			chatHubProxy.On("NotReceivedMessagesChecked", setCheckedNotReceivedMessages);
		}

		private void Connect(HubConnection hubObject, Action<HubConnection> hubConnection)
		{
			hubConnection(hubObject);
		}

		private void SendMessage(IHubProxy hubProxy, Action<IHubProxy> invoke)
		{
			invoke(hubProxy);
		}
		private void MessageReceived(IHubProxy hubProxy, Action<IHubProxy> invoke)
		{
			invoke(hubProxy);
		}

		public void Reconnect()
		{
			Connect(hubConnection, async h =>
			{
				await h.Start();
			});
		}

		public void Dispose()
		{
			hubConnection.Stop();
			serviceInstance = null;
		}


		public void StopConnection()
		{
			hubConnection.Stop();
		}


	}
}
