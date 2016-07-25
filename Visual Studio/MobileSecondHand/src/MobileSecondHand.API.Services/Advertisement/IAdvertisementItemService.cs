using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Advertisement;

namespace MobileSecondHand.API.Services.Advertisement {
	public interface IAdvertisementItemService {
		void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel, string userId);
		Task<IEnumerable<AdvertisementItemShortModel>> GetAdvertisements(SearchAdvertisementsModel searchModel, string userId);
		Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementId, string userId);
	}
}