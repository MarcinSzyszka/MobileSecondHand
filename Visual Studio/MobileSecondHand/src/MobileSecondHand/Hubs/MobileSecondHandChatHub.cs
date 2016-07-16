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

		public void SendMessage(string message, string addresseeId, string conversationId)
		{
			var a = Context;
			var conversationIdParsed = int.Parse(conversationId);
			var senderId = ReadUserIdFromToken();

			var addresseeConnectionIds = this.chatHubCacheService.GetUserConnectionIds(addresseeId);
			var addresseeCanReceiveMessage = addresseeConnectionIds.Count > 0;

			try
			{
				var chatMessage = this.conversationService.AddMessageToConversation(GetMessageSaveModel(conversationIdParsed, senderId, addresseeId, message, addresseeCanReceiveMessage));
				if (addresseeCanReceiveMessage)
				{
					foreach (var connection in addresseeConnectionIds)
					{
						base.Clients.Client(connection).ReceiveMessage(chatMessage.MessageContent, chatMessage.MessageHeader, chatMessage.ConversationId, senderId);
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
			string userId = ReadUserIdFromToken();
			if (userId != String.Empty)
			{
				this.chatHubCacheService.AddConnectedClient(new UserConnection { ConnectionId = Context.ConnectionId, UserId = userId });
			}

			using (var sw = new StreamWriter(@"C:\Users\marcianno\Desktop\logs.txt", true))
			{

				if (userId == "ef15eb21-d31a-4325-bedb-cc8173a98073")
				{
					sw.WriteLine("Htc się podłączył: " + Context.ConnectionId);
				}
				else
				{
					sw.WriteLine("Samsung się podłączył: " + Context.ConnectionId);
				}

				sw.WriteLine();
			}




			return base.OnConnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			this.chatHubCacheService.RemoveDisconnectedClient(Context.ConnectionId);

			return base.OnDisconnected(stopCalled);
		}

		private ChatMessageSaveModel GetMessageSaveModel(int conversationId, string senderId, string addresseeId, string message, bool addresseeCanReceiveMessage)
		{
			return new ChatMessageSaveModel
			{
				ConversationId = conversationId,
				SenderId = senderId,
				AddresseeId = addresseeId,
				Content = message,
				AddresseeCanReceiveMessage = addresseeCanReceiveMessage
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