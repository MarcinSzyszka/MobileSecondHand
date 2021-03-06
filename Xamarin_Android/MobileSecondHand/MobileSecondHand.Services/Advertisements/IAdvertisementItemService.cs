﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;

namespace MobileSecondHand.Services.Advertisements
{
	public interface IAdvertisementItemService
	{
		Task<List<AdvertisementItemShort>> GetAdvertisements(AdvertisementsSearchModel searchModel);
		Task<List<AdvertisementItemShort>> GetUserAdvertisements(int pageNumber, string userId, double lat, double lon);
		Task<AdvertisementItemPhotosNames> UploadNewAdvertisementPhotos(IEnumerable<byte[]> bytesArrayList);
		Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel);
		Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementItemId);
		Task<bool> CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(AdvertisementsSearchModelForNotifications coordinatesMOdel);
		Task<bool> DeleteAdvertisement(int advertisementId, AdvertisementsKind advertisementsKind);
		Task<string> AddToUserFavouritesAdvertisements(SingleIdModelForPostRequests advertisementId);
		Task<bool> RestartAdvertisement(int id);
	}
}
