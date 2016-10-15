/*
 * In the Application_Start method, or somewhere in app's start
 * you'll need to add the namespace inclusion:
 * 
 * using Microsoft.AspNet.SignalR;
 * 
 * and the code:
 * 
 * RouteTable.Routes.MapHubs();
 * 
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using MobileSecondHand.API.Models.Chat;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Services.CacheServices;
using MobileSecondHand.API.Services.Conversation;
using MobileSecondHand.COMMON.Extensions;
using Newtonsoft.Json;

namespace MobileSecondHand.Hubs
{
	[HubName("MobileSecondHandChatHub")]
	public class MobileSecondHandChatHub : Hub
	{
		IChatHubCacheService chatHubCacheService;
		TokenAuthorizationOptions tokenAuthorizationOptions;
		JwtSecurityTokenHandler handler;
		IConversationService conversationService;

		public MobileSecondHandChatHub(IChatHubCacheService chatHubCacheService, TokenAuthorizationOptions tokenAuthorizationOptions, IConversationService conversationService)
		{
			this.chatHubCacheService = chatHubCacheService;
			this.tokenAuthorizationOptions = tokenAuthorizationOptions;
			this.handler = new JwtSecurityTokenHandler();
			this.conversationService = conversationService;
		}

		public override Task OnReconnected()
		{
			//ConnectUser("recconect ");

			return base.OnReconnected();
		}

		public void MessageReceived(string messageIdString)
		{
			var messageId = 0;
			int.TryParse(messageIdString, out messageId);
			if (messageId > 0)
			{
				this.conversationService.MarkMessageAsReceived(messageId);
			}
		}

		public void SendMessage(string message, string addresseeId, string conversationId)
		{
			var a = Context;
			var conversationIdParsed = int.Parse(conversationId);
			var senderId = ReadUserIdFromToken();

			var addresseeConnectionIds = this.chatHubCacheService.GetUserConnectionIds(addresseeId);
			var addresseeIsRegisteredInHub = addresseeConnectionIds.Count > 0;

			try
			{
				var chatMessage = this.conversationService.AddMessageToConversation(GetMessageSaveModel(conversationIdParsed, senderId, addresseeId, message));
				if (addresseeIsRegisteredInHub)
				{
					foreach (var connection in addresseeConnectionIds)
					{
						base.Clients.Client(connection).ReceiveMessage(JsonConvert.SerializeObject(chatMessage));
					}
				}
				else
				{
					//info do nadawca ze odbiorca niedostępny czy cos
				}

			}
			catch (Exception exc)
			{

				throw;
			}
		}

		public override Task OnConnected()
		{
			ConnectUser();

			return base.OnConnected();
		}

		private void ConnectUser(string info = "")
		{
			string userId = ReadUserIdFromToken();
			if (userId != String.Empty)
			{
				this.chatHubCacheService.AddConnectedClient(new UserConnection { ConnectionId = Context.ConnectionId, UserId = userId });
			}

			//using (var sw = new StreamWriter(@"C:\Users\marcianno\Desktop\logs.txt", true))
			//{

			//	if (userId == "ef15eb21-d31a-4325-bedb-cc8173a98073")
			//	{
			//		sw.WriteLine("Htc się podłączył: " + Context.ConnectionId + " " + info);
			//	}
			//	else
			//	{
			//		sw.WriteLine("Samsung się podłączył: " + Context.ConnectionId + " " + info);
			//	}

			//	sw.WriteLine();
			//}

			IEnumerable<ChatMessageReadModel> messages = this.conversationService.GetNotReceivedMessagesAndMarkThemReceived(userId);
			foreach (var chatMessage in messages)
			{
				base.Clients.Client(Context.ConnectionId).ReceiveMessage(JsonConvert.SerializeObject(chatMessage));
			}
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			this.chatHubCacheService.RemoveDisconnectedClient(Context.ConnectionId);

			return base.OnDisconnected(stopCalled);
		}

		private ChatMessageSaveModel GetMessageSaveModel(int conversationId, string senderId, string addresseeId, string message)
		{
			return new ChatMessageSaveModel
			{
				ConversationId = conversationId,
				SenderId = senderId,
				AddresseeId = addresseeId,
				Content = message
			};
		}

		private string ReadUserIdFromToken()
		{
			string userId = String.Empty;
			var token = Context.Headers["Authorization"];
			if (handler.CanReadToken(token))
			{
				var jwtToken = handler.ReadJwtToken(token);
				var userClaim = jwtToken.Claims.Where(c => c.Type == "UserId").FirstOrDefault();
				if (userClaim != null)
				{
					userId = userClaim.Value;
				}
			}

			return userId;
		}
	}
}