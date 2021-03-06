﻿using System.Collections.Generic;
using MobileSecondHand.API.Models.Chat;

namespace MobileSecondHand.API.Services.CacheServices {
	public interface IChatHubCacheService {
		void AddConnectedClient(UserConnection userConnection);
		void RemoveDisconnectedClient(string connectionId);
		bool IsUserConnected(string userId);
		List<string> GetUserConnectionIds(string addresseeId);
	}
}