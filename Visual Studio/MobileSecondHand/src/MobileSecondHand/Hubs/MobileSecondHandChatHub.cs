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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using MobileSecondHand.API.Models.Chat;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Services.CacheServices;

namespace MobileSecondHand.Hubs {
	[HubName("MobileSecondHandChatHub")]
	public class MobileSecondHandChatHub : Hub {
		IChatHubCacheService chatHubCacheService;
		TokenAuthorizationOptions tokenAuthorizationOptions;
		JwtSecurityTokenHandler handler;

		public MobileSecondHandChatHub(IChatHubCacheService chatHubCacheService, TokenAuthorizationOptions tokenAuthorizationOptions) {
			this.chatHubCacheService = chatHubCacheService;
			this.tokenAuthorizationOptions = tokenAuthorizationOptions;
			this.handler = new JwtSecurityTokenHandler();
		}

		public void SendMessage(string message) {
			var a = Context;
			var conversationMessage = 1;
			var senderId = 1;
			base.Clients.All.UpdateChatMessage("Witaj. Pozdro ze strony serwera :)", conversationMessage.ToString(), senderId.ToString());
		}

		public override Task OnConnected() {
			string userId = ReadUserIdFromToken();
			if (userId != String.Empty) {
				this.chatHubCacheService.AddConnectedClient(new UserConnection { ConnectionId = Context.ConnectionId, UserId = userId });
			}


			return base.OnConnected();
		}
		
		public override Task OnDisconnected(bool stopCalled) {
			var a = Context;
			this.chatHubCacheService.RemoveDisconnectedClient(Context.ConnectionId);

			return base.OnConnected();
		}

		private string ReadUserIdFromToken() {
			string userId = String.Empty;
			var token = Context.Headers["Authorization"];
			if (handler.CanReadToken(token)) {
				var jwtToken = handler.ReadJwtToken(token);
				var userClaim = jwtToken.Claims.Where(c => c.Type == "UserId").FirstOrDefault();
				if (userClaim != null) {
					userId = userClaim.Value;
				}
			}

			return userId;
		}
	}
}