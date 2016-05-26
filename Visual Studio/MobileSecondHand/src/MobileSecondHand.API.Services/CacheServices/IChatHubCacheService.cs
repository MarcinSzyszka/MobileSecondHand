using MobileSecondHand.API.Models.ChatHub;

namespace MobileSecondHand.API.Services.CacheServices {
	public interface IChatHubCacheService {
		void AddConnectedClient(UserConnection userConnection);
		void RemoveDisconnectedClient(string connectionId);
	}
}