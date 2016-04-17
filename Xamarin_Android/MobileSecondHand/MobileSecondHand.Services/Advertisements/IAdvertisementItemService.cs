using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Security;

namespace MobileSecondHand.Services.Advertisements {
	public interface IAdvertisementItemService {
		Task<List<AdvertisementItemShort>> GetAdvertisements(SearchModel searchModel, TokenModel tokenModel);
		Task<AdvertisementItemPhotosPaths> UploadNewAdvertisementPhotos(byte[] bytesArray, TokenModel tokenModel);
		Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel, TokenModel tokenModel);
	}
}
