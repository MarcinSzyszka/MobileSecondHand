using MobileSecondHand.Api.Models.ChatHub;

namespace MobileSecondHand.Api.Services.CacheServices {
	public interface IChatHubCacheService {
		void AddConnectedClient(UserConnection userConnection);
		void RemoveDisconnectedClient(string connectionId);
	}
}