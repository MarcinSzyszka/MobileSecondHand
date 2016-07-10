using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using MobileSecondHand.API.Models.Chat;

namespace MobileSecondHand.API.Services.CacheServices
{
	public class ChatHubCacheService : IChatHubCacheService
	{
		MemoryCache cache;
		const string CONNECTED_USERS = "CONNECTED_USERS";
		public ChatHubCacheService()
		{
			this.cache = MemoryCache.Default;
		}

		public void AddConnectedClient(UserConnection userConnection)
		{
			var connectedUsers = GetConnectedUsers();
			if (connectedUsers == null)
			{
				var newConnectedUsersList = new List<UserConnection> { userConnection };
				this.cache.Set(CONNECTED_USERS, newConnectedUsersList, ObjectCache.InfiniteAbsoluteExpiration);
			}
			else
			{
				connectedUsers.Add(userConnection);

			}
		}

		public void RemoveDisconnectedClient(string connectionId)
		{
			var connectedUsers = GetConnectedUsers();
			if (connectedUsers != null)
			{
				var disconnectedUser = connectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId);
				if (disconnectedUser != null)
				{

					using (var sw = new StreamWriter(@"C:\Users\marcianno\Desktop\logs.txt", true))
					{

						if (disconnectedUser.UserId == "ef15eb21-d31a-4325-bedb-cc8173a98073")
						{
							sw.WriteLine("Htc się rozłączył: " + connectionId);
						}
						else
						{
							sw.WriteLine("Samsung się rozłączył: " + connectionId);
						}

						sw.WriteLine();
					}


					connectedUsers.Remove(disconnectedUser);
				}
			}
		}

		public List<string> GetUserConnectionIds(string addresseeId)
		{
			var users = GetConnectedUsers();

			var userConnections = users.Where(c => c.UserId == addresseeId).Select(c => c.ConnectionId).ToList();

			return userConnections;
		}

		public bool IsUserConnected(string userId)
		{
			var userConnection = GetConnectedUsers().FirstOrDefault(u => u.UserId == userId);

			return userConnection != null;
		}

		private List<UserConnection> GetConnectedUsers()
		{
			return (List<UserConnection>)this.cache.Get(CONNECTED_USERS);
		}
	}
}
