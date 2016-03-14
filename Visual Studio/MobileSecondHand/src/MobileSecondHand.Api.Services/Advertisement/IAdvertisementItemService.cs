using System.Collections.Generic;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Api.Models.Coordinates;

namespace MobileSecondHand.Api.Services.Advertisement {
	public interface IAdvertisementItemService {
		void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel);
		IEnumerable<AdvertisementItemShortModel> GetAdvertisements(CoordinatesModel coordinatesModel);
	}
}