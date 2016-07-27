using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Security;

namespace MobileSecondHand.Services.Advertisements {
	public interface IAdvertisementItemService {
		Task<List<AdvertisementItemShort>> GetAdvertisements(SearchAdvertisementsModel searchModel);
		Task<List<AdvertisementItemShort>> GetUserAdvertisements(int pageNumber);
		Task<AdvertisementItemPhotosPaths> UploadNewAdvertisementPhotos(byte[] bytesArray);
		Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel);
		Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementItemId);
		
	}
}
