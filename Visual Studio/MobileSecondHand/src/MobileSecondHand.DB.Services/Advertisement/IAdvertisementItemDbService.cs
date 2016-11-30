using System;
using System.Collections.Generic;
using System.Linq;
using MobileSecondHand.COMMON.Models;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Services.Advertisement {
	public interface IAdvertisementItemDbService
    {
		void SaveNewAdvertisementItem(AdvertisementItem advertisementItem);
		IQueryable<AdvertisementItem> GetAdvertisements();
		AdvertisementItem GetByIdWithDetails(int advertisementId);
		IQueryable<AdvertisementItem> GetUserAdvertisements(string userId, int pageNumber = -1);
		IQueryable<AdvertisementItem> GetAdvertisementsFromDeclaredAreaSinceLastCheck(DateTime lastCheckDate, string userId, CoordinatesForSearchingAdvertisementsModel coordinatesForSearchModel);
		AdvertisementItem GetById(int advertisementId);
		AdvertisementItem GetByIdWithPhotos(int advertisementId);
		void SaveAdvertisementItem(AdvertisementItem advertisement);
		UserToFavouriteAdvertisement GetUserFavouriteAdvertisement(string userId, int advertisementId);
		void SaveUserFavouriteAdvertisement(UserToFavouriteAdvertisement favouriteAdvertisement);
		IEnumerable<UserToFavouriteAdvertisement> GetUserFavouritesAdvertisements(string userId);
		void DeleteFavouriteAdvertisement(UserToFavouriteAdvertisement advertisement);
	
	}
}
