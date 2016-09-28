using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Location;
using MobileSecondHand.Models.Security;

namespace MobileSecondHand.Services.Advertisements {
	public interface IAdvertisementItemService {
		Task<List<AdvertisementItemShort>> GetAdvertisements(SearchAdvertisementsModel searchModel);
		Task<List<AdvertisementItemShort>> GetUserAdvertisements(int pageNumber);
		Task<AdvertisementItemPhotosPaths> UploadNewAdvertisementPhotos(IEnumerable<byte[]> bytesArrayList);
		Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel);
		Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementItemId);
		Task<bool> CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(CoordinatesForAdvertisementsModel coordinatesMOdel);
		Task<bool> DeleteAdvertisement(int advertisementId);
		Task<string> AddToUserFavouritesAdvertisements(SingleIdModelForPostRequests advertisementId);
	}
}
