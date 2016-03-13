using MobileSecondHand.Api.Models.Advertisement;

namespace MobileSecondHand.Api.Services.Advertisement {
	public interface IAdvertisementItemService {
		void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel);
	}
}