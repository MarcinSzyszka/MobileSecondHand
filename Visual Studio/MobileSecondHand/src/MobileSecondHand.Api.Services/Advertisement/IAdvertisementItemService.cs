using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Api.Models.Coordinates;

namespace MobileSecondHand.Api.Services.Advertisement {
	public interface IAdvertisementItemService {
		void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel, string userId);
		Task<IEnumerable<AdvertisementItemShortModel>> GetAdvertisements(CoordinatesModel coordinatesModel, string userId);
	}
}