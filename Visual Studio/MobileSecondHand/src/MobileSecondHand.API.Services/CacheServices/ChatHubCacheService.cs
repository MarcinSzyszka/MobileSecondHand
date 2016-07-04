using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using MobileSecondHand.API.Models.Chat;

namespace MobileSecondHand.API.Services.CacheServices {
	public class ChatHubCacheService : IChatHubCacheService {
		MemoryCache cache;
		const string CONNECTED_USERS = "CONNECTED_USERS";
		public ChatHubCacheService() {
			this.cache = MemoryCache.Default;
		}

		public void AddConnectedClient(UserConnection userConnection) {
			var connectedUsers = GetConnectedUsers();
			if (connectedUsers == null) {
				var newConnectedUsersList = new List<UserConnection> { userConnection };
				this.cache.Set(CONNECTED_USERS, newConnectedUsersList, ObjectCache.InfiniteAbsoluteExpiration);
			}
			else {
				connectedUsers.Add(userConnection);
				this.cache.Set(CONNECTED_USERS, connectedUsers, ObjectCache.InfiniteAbsoluteExpiration);
			}
		}

		public void RemoveDisconnectedClient(string connectionId) {
			var connectedUsers = GetConnectedUsers();
			if (connectedUsers != null) {
				var disconnectedUser = connectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId);
				if (disconnectedUser != null) {
					connectedUsers.Remove(disconnectedUser);
					this.cache.Set(CONNECTED_USERS, connectedUsers, ObjectCache.InfiniteAbsoluteExpiration);
				}
			}
		}

		public string GetUserConnectionId(string addresseeId)
		{
			var users = GetConnectedUsers();

			var userConnection = users.FirstOrDefault(c => c.UserId == addresseeId);
			if (userConnection != null)
			{
				return userConnection.ConnectionId;
			}

			return string.Empty;
		}

		public bool IsUserConnected(string userId)
		{
			var userConnection = GetConnectedUsers().FirstOrDefault(u => u.UserId == userId);

			return userConnection != null;
		}

		private List<UserConnection> GetConnectedUsers() {
			return (List<UserConnection>)this.cache.Get(CONNECTED_USERS);
		}		
	}
}
