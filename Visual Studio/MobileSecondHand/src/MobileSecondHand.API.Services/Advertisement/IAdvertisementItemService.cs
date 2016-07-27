using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Advertisement;
using MobileSecondHand.API.Models.Coordinates;

namespace MobileSecondHand.API.Services.Advertisement {
	public interface IAdvertisementItemService {
		void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel, string userId);
		Task<IEnumerable<AdvertisementItemShortModel>> GetAdvertisements(SearchAdvertisementsModel searchModel, string userId);
		Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementId, string userId);
		Task<IEnumerable<AdvertisementItemShortModel>> GetUserAdvertisements(string userId, int pageNumber);
		bool CheckForNewAdvertisementsSinceLastCheck(string userId, CoordinatesForAdvertisementsModel coordinatesModel);
	}
}