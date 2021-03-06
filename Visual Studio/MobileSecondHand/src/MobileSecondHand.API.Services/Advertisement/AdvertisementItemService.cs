﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Services.Keywords;
using MobileSecondHand.API.Services.CacheServices;
using MobileSecondHand.COMMON.CoordinatesHelpers;
using MobileSecondHand.COMMON.PathHelpers;
using MobileSecondHand.DB.Models.Advertisement;
using MobileSecondHand.DB.Services.Advertisement;
using MobileSecondHand.DB.Models.Keywords;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Consts;
using MobileSecondHand.API.Services.Photos;
using MobileSecondHand.DB.Models.Feedback;
using MobileSecondHand.API.Services.Shared.GoogleApi;

namespace MobileSecondHand.API.Services.Advertisement
{
	public class AdvertisementItemService : IAdvertisementItemService
	{
		IAdvertisementItemDbService advertisementItemDbService;
		ICoordinatesCalculator coordinatesCalculator;
		IPhotosService photosService;
		IAppFilesPathHelper appFilesPathHelper;
		IKeywordsService keywordsService;
		IChatHubCacheService chatHubCacheService;
		ILastUsersChecksCacheService lastUsersChecksCacheService;
		IGoogleMapsAPIService googleMapsAPIService;
		const int ITEMS_PER_REQUEST = 20;

		public AdvertisementItemService(IAdvertisementItemDbService advertisementItemDbService, ICoordinatesCalculator coordinatesCalculator, IPhotosService photosService, IAppFilesPathHelper appFilesPathHelper, IKeywordsService keywordsService, IChatHubCacheService chatHubCacheService, ILastUsersChecksCacheService lastUsersChecksCacheService, IGoogleMapsAPIService googleMapsAPIService)
		{
			this.advertisementItemDbService = advertisementItemDbService;
			this.coordinatesCalculator = coordinatesCalculator;
			this.photosService = photosService;
			this.appFilesPathHelper = appFilesPathHelper;
			this.keywordsService = keywordsService;
			this.chatHubCacheService = chatHubCacheService;
			this.lastUsersChecksCacheService = lastUsersChecksCacheService;
			this.googleMapsAPIService = googleMapsAPIService;
		}

		public void CreateNewAdvertisementItem(NewAdvertisementItem newAdvertisementModel, string userId)
		{
			AdvertisementItem model = null;
			if (newAdvertisementModel.Id > 0)
			{
				model = this.advertisementItemDbService.GetByIdWithPhotos(newAdvertisementModel.Id);
				model.Title = newAdvertisementModel.AdvertisementTitle;
				model.Description = newAdvertisementModel.AdvertisementDescription;
				model.Size = newAdvertisementModel.Size;
				model.Price = newAdvertisementModel.AdvertisementPrice;
				model.ExpirationDate = GetExpirationDate();
				model.Latitude = newAdvertisementModel.Latitude;
				model.Longitude = newAdvertisementModel.Longitude;
				model.IsOnlyForSell = newAdvertisementModel.IsOnlyForSell;
				model.CategoryId = newAdvertisementModel.Category.Id;
				model.AdvertisementPhotos.AddRange(CreateAdvertisementPhotosModels(newAdvertisementModel.PhotosNames));

			}
			else
			{
				model = new AdvertisementItem
				{
					UserId = userId,
					Title = newAdvertisementModel.AdvertisementTitle,
					Description = newAdvertisementModel.AdvertisementDescription,
					Size = newAdvertisementModel.Size,
					Price = newAdvertisementModel.AdvertisementPrice,
					CreationDate = DateTime.Now,
					ExpirationDate = GetExpirationDate(),
					Latitude = newAdvertisementModel.Latitude,
					Longitude = newAdvertisementModel.Longitude,
					IsOnlyForSell = newAdvertisementModel.IsOnlyForSell,
					CategoryId = newAdvertisementModel.Category.Id,
					AdvertisementPhotos = CreateAdvertisementPhotosModels(newAdvertisementModel.PhotosNames)
				};
			}

			this.advertisementItemDbService.SaveNewAdvertisementItem(model);
		}


		public async Task<IEnumerable<AdvertisementItemShort>> GetAdvertisements(AdvertisementsSearchModel searchModel, string userId)
		{
			IQueryable<AdvertisementItem> queryAdvertisements = default(IQueryable<AdvertisementItem>);
			switch (searchModel.AdvertisementsKind)
			{
				case AdvertisementsKind.AdvertisementsAroundUserCurrentLocation:
				case AdvertisementsKind.AdvertisementsArounUserHomeLocation:
					queryAdvertisements = this.advertisementItemDbService.GetAdvertisements();
					break;
				case AdvertisementsKind.AdvertisementsCreatedByUser:
					queryAdvertisements = this.advertisementItemDbService.GetUserAdvertisements(userId);
					break;
				case AdvertisementsKind.FavouritesAdvertisements:
					queryAdvertisements = this.advertisementItemDbService.GetUserFavouritesAdvertisements(userId).ToList().Select(a => a.AdvertisementItem).AsQueryable();
					break;
				default:
					break;
			}

			queryAdvertisements = FilterResultBySearchModelOptions(searchModel, queryAdvertisements);

			IEnumerable<AdvertisementItemShort> advertisementsViewModels = await MapDbModelsToShortViewModels(queryAdvertisements.ToList(), searchModel.CoordinatesModel);

			UpdateUserCheckingList(searchModel, userId);

			return advertisementsViewModels;
		}

		private void UpdateUserCheckingList(AdvertisementsSearchModel searchModel, string userId)
		{
			if (searchModel.AdvertisementsKind == AdvertisementsKind.AdvertisementsAroundUserCurrentLocation || searchModel.AdvertisementsKind == AdvertisementsKind.AdvertisementsArounUserHomeLocation)
			{
				this.lastUsersChecksCacheService.UpdateLastTimeUserCheckDate(userId, searchModel.AdvertisementsKind == AdvertisementsKind.AdvertisementsAroundUserCurrentLocation);
			}
		}

		private IQueryable<AdvertisementItem> FilterResultBySearchModelOptions(AdvertisementsSearchModel searchModel, IQueryable<AdvertisementItem> queryAdvertisements)
		{
			//---------ExpiredStatus-------------------
			if (searchModel.ExpiredAdvertisements)
			{
				queryAdvertisements = queryAdvertisements.Where(a => a.ExpirationDate <= DateTime.Now);
			}
			else
			{
				queryAdvertisements = queryAdvertisements.Where(a => a.ExpirationDate >= DateTime.Now);
			}
			//---------TransactionKind-----------
			if (searchModel.TransactionKind == TransactionKind.OnlyWithChange)
			{
				queryAdvertisements = queryAdvertisements.Where(a => !a.IsOnlyForSell);
			}
			else if (searchModel.TransactionKind == TransactionKind.OnlyWithSell)
			{
				queryAdvertisements = queryAdvertisements.Where(a => a.IsOnlyForSell);
			}

			//---------Categories-----------
			if (searchModel.CategoriesModel.Count > 0)
			{
				var categoriesIds = searchModel.CategoriesModel.Select(c => c.Key).ToList();
				queryAdvertisements = queryAdvertisements.Where(a => categoriesIds.Contains(a.CategoryId));
			}

			//---------Sizes-----------
			if (searchModel.Sizes.Count > 0)
			{
				queryAdvertisements = queryAdvertisements.Where(a => searchModel.Sizes.Contains(a.Size));
			}

			//---------SelectedUser-----------
			if (searchModel.UserInfo != null)
			{
				queryAdvertisements = queryAdvertisements.Where(a => a.UserId == searchModel.UserInfo.Id);
			}

			//---------Coordinates-----------
			if (searchModel.CoordinatesModel.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE)
			{
				var coordinatesForSearchModel = coordinatesCalculator.GetCoordinatesForSearchingAdvertisements(searchModel.CoordinatesModel.Latitude, searchModel.CoordinatesModel.Longitude, searchModel.CoordinatesModel.MaxDistance);
				queryAdvertisements = queryAdvertisements.Where(a => a.Latitude >= coordinatesForSearchModel.LatitudeStart
																	&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
																	&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
																	&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd);
			}


			//---------SortingBy-----------
			queryAdvertisements = SortQuery(queryAdvertisements, searchModel);

			queryAdvertisements = queryAdvertisements.Skip(ITEMS_PER_REQUEST * searchModel.Page).Take(ITEMS_PER_REQUEST);
			return queryAdvertisements;
		}


		public async Task<IEnumerable<AdvertisementItemShort>> GetUserAdvertisements(string userId, int pageNumber, double lat, double lon)
		{
			var coordinatesModel = new CoordinatesForAdvertisementsModel { Latitude = lat, Longitude = lon };
			var advertisementsFromDb = this.advertisementItemDbService.GetUserAdvertisements(userId, pageNumber).ToList();
			IEnumerable<AdvertisementItemShort> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb, coordinatesModel);

			return advertisementsViewModels;
		}


		public async Task<IEnumerable<AdvertisementItemShort>> GetUserAdvertisements(string userId, int pageNumber)
		{
			var advertisementsFromDb = this.advertisementItemDbService.GetUserAdvertisements(userId, pageNumber).ToList();
			IEnumerable<AdvertisementItemShort> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb);

			return advertisementsViewModels;
		}

		public bool CheckForNewAdvertisementsSinceLastCheck(string userId, AdvertisementsSearchModelForNotifications searchModel, bool currentLocation)
		{
			var coordinatesForSearchModel = coordinatesCalculator.GetCoordinatesForSearchingAdvertisements(searchModel.CoordinatesModels.Latitude, searchModel.CoordinatesModels.Longitude, searchModel.CoordinatesModels.MaxDistance);
			var lastUserCheckModel = this.lastUsersChecksCacheService.GetLastTimeUserCheck(userId);
			var dateToCompare = currentLocation ? lastUserCheckModel.LastAroundCurrentLocationCheckDate : lastUserCheckModel.LastAroundHomeLocationCheckDate;
			var advertisementsFromDbQuery = this.advertisementItemDbService.GetAdvertisementsFromDeclaredAreaSinceLastCheck(dateToCompare, userId, coordinatesForSearchModel);
			if (searchModel.CategoriesIds.Count > 0)
			{
				advertisementsFromDbQuery = advertisementsFromDbQuery.Where(a => searchModel.CategoriesIds.Contains(a.CategoryId));
			}
			if (searchModel.Sizes.Count > 0)
			{
				advertisementsFromDbQuery = advertisementsFromDbQuery.Where(a => searchModel.Sizes.Contains(a.Size));
			}
			var advertisements = advertisementsFromDbQuery.Take(1).ToList();
			this.lastUsersChecksCacheService.UpdateLastTimeUserCheckDate(userId, currentLocation);

			return advertisements.Count > 0;
		}

		public async Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementId, string userId)
		{
			var advertisementDetailsViewModel = default(AdvertisementItemDetails);
			var advertisementFromDb = this.advertisementItemDbService.GetByIdWithDetails(advertisementId);
			if (advertisementFromDb != null)
			{
				try
				{
					await UpdateCityNameIfDoesNotSetYet(advertisementFromDb);
				}
				catch (Exception exc)
				{
					//wyjebało sie cos przy sprawdzaniu nazwy miasta
				}

				advertisementDetailsViewModel = await MapToDetailsViewModel(advertisementFromDb);
			}

			return advertisementDetailsViewModel;
		}

		public bool RestartAdvertisement(int advertisementId, string userId)
		{
			AdvertisementItem advertisement = this.advertisementItemDbService.GetById(advertisementId);
			if (advertisement == null)
			{
				throw new Exception("Nie znaleziono ogłoszenia o podanym ID");
			}

			if (advertisement.UserId != userId)
			{
				throw new Exception("Próba restartu ogłoszenia przez nieuprawnionego usera");
			}

			advertisement.ExpirationDate = GetExpirationDate();

			this.advertisementItemDbService.SaveAdvertisementItem(advertisement);

			return true;
		}

		public bool DeleteAdvertisement(int advertisementId, string userId)
		{
			AdvertisementItem advertisement = this.advertisementItemDbService.GetById(advertisementId);
			if (advertisement == null)
			{
				throw new Exception("Nie znaleziono ogłoszenia o podanym ID");
			}

			if (advertisement.UserId != userId)
			{
				throw new Exception("Próba usunięcia ogłoszenia przez nieuprawnionego usera");
			}

			advertisement.ExpirationDate = DateTime.Now;

			this.advertisementItemDbService.SaveAdvertisementItem(advertisement);

			return true;
		}

		public bool DeleteAdvertisementFromFavourites(int advertisementId, string userId)
		{
			var advertisement = this.advertisementItemDbService.GetUserFavouriteAdvertisement(userId, advertisementId);
			if (advertisement == null)
			{
				throw new Exception("Nie znaleziono ogłoszenia o podanym ID");
			}

			if (advertisement.ApplicationUserId != userId)
			{
				throw new Exception("Próba usunięcia ogłoszenia przez nieuprawnionego usera");
			}

			this.advertisementItemDbService.DeleteFavouriteAdvertisement(advertisement);

			return true;
		}

		public bool AddToUserFavourites(string userId, int advertisementId)
		{
			UserToFavouriteAdvertisement favouriteAdvertisement = this.advertisementItemDbService.GetUserFavouriteAdvertisement(userId, advertisementId);
			if (favouriteAdvertisement != null)
			{
				//user already has this advert in his favourites
				return false;
			}

			favouriteAdvertisement = new UserToFavouriteAdvertisement { AdvertisementItemId = advertisementId, ApplicationUserId = userId };
			this.advertisementItemDbService.SaveUserFavouriteAdvertisement(favouriteAdvertisement);

			return true;
		}


		private IQueryable<AdvertisementItem> SortQuery(IQueryable<AdvertisementItem> queryAdvertisements, AdvertisementsSearchModel searchModel)
		{
			switch (searchModel.SortingBy)
			{
				case SortingBy.sortByNearest:
					return queryAdvertisements.OrderBy(a => this.coordinatesCalculator.GetDistanceBetweenTwoLocalizations(searchModel.CoordinatesModel.Latitude, searchModel.CoordinatesModel.Longitude, a.Latitude, a.Longitude));
				case SortingBy.sortByFarthest:
					return queryAdvertisements.OrderByDescending(a => this.coordinatesCalculator.GetDistanceBetweenTwoLocalizations(searchModel.CoordinatesModel.Latitude, searchModel.CoordinatesModel.Longitude, a.Latitude, a.Longitude));
				case SortingBy.sortByLowestPrice:
					return queryAdvertisements.OrderBy(a => a.Price);
				case SortingBy.sortByHighestPrice:
					return queryAdvertisements.OrderByDescending(a => a.Price);
				case SortingBy.sortByNewest:
					return queryAdvertisements.OrderByDescending(a => a.CreationDate);
				default:
					return queryAdvertisements;
			}
		}

		private async Task<AdvertisementItemDetails> MapToDetailsViewModel(AdvertisementItem advertisementFromDb)
		{
			var viewModel = new AdvertisementItemDetails();

			viewModel.Id = advertisementFromDb.Id;
			viewModel.CityName = advertisementFromDb.CityName != "brak" ? advertisementFromDb.CityName : String.Empty;
			viewModel.Title = advertisementFromDb.Title;
			viewModel.Description = advertisementFromDb.Description;
			viewModel.Size = advertisementFromDb.Size;
			viewModel.CategoryInfoModel = new Models.Shared.Categories.CategoryInfoModel { Id = advertisementFromDb.CategoryId, Name = advertisementFromDb.Category.Name };
			viewModel.Price = advertisementFromDb.Price;
			viewModel.IsOnlyForSell = advertisementFromDb.IsOnlyForSell;
			viewModel.SellerId = advertisementFromDb.UserId;
			viewModel.SellerName = advertisementFromDb.User.UserName;
			viewModel.IsActive = advertisementFromDb.ExpirationDate > DateTime.Now;
			viewModel.ExpirationDate = advertisementFromDb.ExpirationDate.Value;
			viewModel.Photos = await GetPhotosList(advertisementFromDb.AdvertisementPhotos.Where(p => !p.IsMainPhoto).ToList());
			viewModel.IsSellerOnline = this.chatHubCacheService.IsUserConnected(advertisementFromDb.UserId);
			if (!String.IsNullOrEmpty(advertisementFromDb.User.UserProfilePhotoName))
			{
				viewModel.SellerProfileImage = await photosService.GetUserProfilePhotoInBytes(advertisementFromDb.User.UserProfilePhotoName);
			}
			return viewModel;
		}

		private async Task UpdateCityNameIfDoesNotSetYet(AdvertisementItem advertisementFromDb)
		{
			if (String.IsNullOrEmpty(advertisementFromDb.CityName))
			{
				var city = await googleMapsAPIService.GetCity(advertisementFromDb.Latitude, advertisementFromDb.Longitude);
				if (!String.IsNullOrEmpty(city))
				{
					advertisementFromDb.CityName = city;
				}
				else
				{
					advertisementFromDb.CityName = "brak";
				}

				this.advertisementItemDbService.SaveAdvertisementItem(advertisementFromDb);
			}
		}

		private async Task<List<byte[]>> GetPhotosList(List<AdvertisementPhoto> photos)
		{
			var photosList = new List<byte[]>();
			foreach (var photo in photos)
			{
				photosList.Add(await this.photosService.GetAdvertisementMainPhotoInBytes(photo.PhotoName));
			}

			return photosList;
		}

		private async Task<IEnumerable<AdvertisementItemShort>> MapDbModelsToShortViewModels(IEnumerable<AdvertisementItem> advertisementsFromDb, CoordinatesForAdvertisementsModel coordinatesModel = null)
		{
			var viewModelsList = new List<AdvertisementItemShort>();
			foreach (var dbModel in advertisementsFromDb)
			{
				var viewModel = new AdvertisementItemShort();
				viewModel.Id = dbModel.Id;
				viewModel.Size = dbModel.Size;
				viewModel.AdvertisementTitle = dbModel.Title;
				viewModel.AdvertisementPrice = dbModel.Price;
				viewModel.MainPhoto = await this.photosService.GetAdvertisementMinPhotoInBytes(dbModel.AdvertisementPhotos.FirstOrDefault(p => p.IsMainPhoto).PhotoName);
				if (coordinatesModel != null)
				{
					viewModel.Distance = this.coordinatesCalculator.GetDistanceBetweenTwoLocalizations(coordinatesModel.Latitude, coordinatesModel.Longitude, dbModel.Latitude, dbModel.Longitude);
				}
				else
				{
					viewModel.Distance = 0.0D;
				}
				viewModel.IsSellerOnline = this.chatHubCacheService.IsUserConnected(dbModel.UserId);
				viewModel.IsOnlyForSell = dbModel.IsOnlyForSell;
				viewModel.IsExpired = dbModel.ExpirationDate < DateTime.Now;
				viewModelsList.Add(viewModel);
			}

			return viewModelsList;
		}

		private List<AdvertisementPhoto> CreateAdvertisementPhotosModels(IEnumerable<string> advertisementPhotosNames)
		{
			var photosDbModelsList = new List<AdvertisementPhoto>();
			foreach (var photoName in advertisementPhotosNames)
			{
				var model = new AdvertisementPhoto();
				model.PhotoName = photoName;
				model.IsMainPhoto = photoName.StartsWith("mini");
				photosDbModelsList.Add(model);
			}

			return photosDbModelsList;
		}

		private DateTime GetExpirationDate()
		{
			return DateTime.Now.AddDays(14);
		}


	}
}
