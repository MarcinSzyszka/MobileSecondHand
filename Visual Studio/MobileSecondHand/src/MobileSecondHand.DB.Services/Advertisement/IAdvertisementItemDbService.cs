using System;
using System.Collections.Generic;
using MobileSecondHand.COMMON.Models;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Services.Advertisement {
	public interface IAdvertisementItemDbService
    {
		void SaveNewAdvertisementItem(AdvertisementItem advertisementItem);
		IEnumerable<AdvertisementItem> GetAdvertisementsFromDeclaredArea(CoordinatesForSearchingAdvertisementsModel coordinatesForSearchModel, int page);
		AdvertisementItem GetByIdWithDetails(int advertisementId);
		IEnumerable<AdvertisementItem> GetUserAdvertisements(string userId, int pageNumber);
		IEnumerable<AdvertisementItem> GetAdvertisementsFromDeclaredAreaSinceLastCheck(DateTime lastCheckDate, string userId, CoordinatesForSearchingAdvertisementsModel coordinatesForSearchModel);
		AdvertisementItem GetById(int advertisementId);
		void SaveAdvertisementItem(AdvertisementItem advertisement);
		UserToFavouriteAdvertisement GetUserFavouriteAdvertisement(string userId, int advertisementId);
		void SaveUserFavouriteAdvertisement(UserToFavouriteAdvertisement favouriteAdvertisement);
	}
}
