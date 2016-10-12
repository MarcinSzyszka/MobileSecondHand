using System;
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

namespace MobileSecondHand.API.Services.Advertisement
{
	public class AdvertisementItemService : IAdvertisementItemService
	{
		IAdvertisementItemDbService advertisementItemDbService;
		ICoordinatesCalculator coordinatesCalculator;
		IAdvertisementItemPhotosService advertisementItemPhotosService;
		IAppFilesPathHelper appFilesPathHelper;
		IKeywordsService keywordsService;
		IChatHubCacheService chatHubCacheService;
		ILastUsersChecksCacheService lastUsersChecksCacheService;
		const int ITEMS_PER_REQUEST = 20;

		public AdvertisementItemService(IAdvertisementItemDbService advertisementItemDbService, ICoordinatesCalculator coordinatesCalculator, IAdvertisementItemPhotosService advertisementItemPhotosService, IAppFilesPathHelper appFilesPathHelper, IKeywordsService keywordsService, IChatHubCacheService chatHubCacheService, ILastUsersChecksCacheService lastUsersChecksCacheService)
		{
			this.advertisementItemDbService = advertisementItemDbService;
			this.coordinatesCalculator = coordinatesCalculator;
			this.advertisementItemPhotosService = advertisementItemPhotosService;
			this.appFilesPathHelper = appFilesPathHelper;
			this.keywordsService = keywordsService;
			this.chatHubCacheService = chatHubCacheService;
			this.lastUsersChecksCacheService = lastUsersChecksCacheService;
		}

		public void CreateNewAdvertisementItem(NewAdvertisementItem newAdvertisementModel, string userId)
		{
			var advertisementItemDescription = String.Concat(newAdvertisementModel.AdvertisementTitle, ' ', newAdvertisementModel.AdvertisementDescription);
			var categoryKeywords = this.keywordsService.RecognizeAndGetKeywordsDbModels<CategoryKeyword>(advertisementItemDescription);
			var colorKeywords = this.keywordsService.RecognizeAndGetKeywordsDbModels<ColorKeyword>(advertisementItemDescription);

			var model = new AdvertisementItem
			{
				UserId = userId,
				Title = newAdvertisementModel.AdvertisementTitle,
				Description = newAdvertisementModel.AdvertisementDescription,
				Price = newAdvertisementModel.AdvertisementPrice,
				IsActive = true,
				CreationDate = DateTime.Now,
				ExpirationDate = DateTime.Now.AddDays(7),
				Latitude = newAdvertisementModel.Latitude,
				Longitude = newAdvertisementModel.Longitude,
				IsOnlyForSell = newAdvertisementModel.IsOnlyForSell,
				CategoryId = newAdvertisementModel.Category.Id,
				AdvertisementPhotos = CreateAdvertisementPhotosModels(newAdvertisementModel.PhotosPaths)
			};

			foreach (var category in categoryKeywords)
			{
				model.CategoryKeywords.Add(new CategoryKeywordToAdvertisement
				{
					AdvertisementItem = model,
					CategoryKeyword = category
				});
			}

			foreach (var color in colorKeywords)
			{
				model.ColorKeywords.Add(new ColorKeywordToAdvertisement { AdvertisementItem = model, ColorKeyword = color });
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
					var coordinatesForSearchModel = coordinatesCalculator.GetCoordinatesForSearchingAdvertisements(searchModel.CoordinatesModel.Latitude, searchModel.CoordinatesModel.Longitude, searchModel.CoordinatesModel.MaxDistance);
					queryAdvertisements = this.advertisementItemDbService.GetAdvertisementsFromDeclaredArea(coordinatesForSearchModel, searchModel.Page);
					break;
				case AdvertisementsKind.AdvertisementsCreatedByUser:
					queryAdvertisements = this.advertisementItemDbService.GetUserAdvertisements(userId, searchModel.Page);
					break;
				case AdvertisementsKind.FavouritesAdvertisements:
					queryAdvertisements = this.advertisementItemDbService.GetUserFavouritesAdvertisements(userId, searchModel.Page).ToList().Select(a => a.AdvertisementItem).AsQueryable();
					break;
				default:
					break;
			}

			if (searchModel.CategoriesModel.Count > 0)
			{
				var categoriesIds = searchModel.CategoriesModel.Select(c => c.Key).ToList();
				queryAdvertisements = queryAdvertisements.Where(a => categoriesIds.Contains(a.CategoryId));
			}

			queryAdvertisements = queryAdvertisements.Skip(ITEMS_PER_REQUEST * searchModel.Page).Take(ITEMS_PER_REQUEST);

			IEnumerable<AdvertisementItemShort> advertisementsViewModels = await MapDbModelsToShortViewModels(queryAdvertisements.ToList(), searchModel.CoordinatesModel);

			return advertisementsViewModels;
		}

		public async Task<IEnumerable<AdvertisementItemShort>> GetUserAdvertisements(string userId, int pageNumber)
		{
			var advertisementsFromDb = this.advertisementItemDbService.GetUserAdvertisements(userId, pageNumber).ToList();
			IEnumerable<AdvertisementItemShort> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb);

			return advertisementsViewModels;
		}

		public async Task<IEnumerable<AdvertisementItemShort>> GetUserFavouritesAdvertisements(string userId, int pageNumber)
		{
			var advertisementsFromDb = this.advertisementItemDbService.GetUserFavouritesAdvertisements(userId, pageNumber).ToList();
			IEnumerable<AdvertisementItemShort> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb.Select(a => a.AdvertisementItem));

			return advertisementsViewModels;
		}

		public bool CheckForNewAdvertisementsSinceLastCheck(string userId, CoordinatesForAdvertisementsModel coordinatesModel, bool currentLocation)
		{
			var coordinatesForSearchModel = coordinatesCalculator.GetCoordinatesForSearchingAdvertisements(coordinatesModel.Latitude, coordinatesModel.Longitude, coordinatesModel.MaxDistance);
			var lastUserCheckModel = this.lastUsersChecksCacheService.GetLastTimeUserCheck(userId);
			var dateToCompare = currentLocation ? lastUserCheckModel.LastAroundCurrentLocationCheckDate : lastUserCheckModel.LastAroundHomeLocationCheckDate;
			var advertisementsFromDb = this.advertisementItemDbService.GetAdvertisementsFromDeclaredAreaSinceLastCheck(dateToCompare, userId, coordinatesForSearchModel).ToList();
			this.lastUsersChecksCacheService.UpdateLastTimeUserCheckDate(userId, currentLocation);

			return advertisementsFromDb.Count > 0;
		}

		public async Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementId, string userId)
		{
			var advertisementDetailsViewModel = default(AdvertisementItemDetails);
			var advertisementFromDb = this.advertisementItemDbService.GetByIdWithDetails(advertisementId);
			if (advertisementFromDb != null)
			{
				advertisementDetailsViewModel = await MapToDetailsViewModel(advertisementFromDb);
			}

			return advertisementDetailsViewModel;
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

		private async Task<AdvertisementItemDetails> MapToDetailsViewModel(AdvertisementItem advertisementFromDb)
		{
			var viewModel = new AdvertisementItemDetails();
			viewModel.Id = advertisementFromDb.Id;
			viewModel.Title = advertisementFromDb.Title;
			viewModel.Description = advertisementFromDb.Description;
			viewModel.Price = advertisementFromDb.Price;
			viewModel.IsOnlyForSell = advertisementFromDb.IsOnlyForSell;
			viewModel.SellerId = advertisementFromDb.UserId;
			viewModel.SellerName = advertisementFromDb.User.UserName;
			viewModel.Photos = await GetPhotosList(advertisementFromDb.AdvertisementPhotos.Where(p => !p.IsMainPhoto).ToList());
			viewModel.IsSellerOnline = this.chatHubCacheService.IsUserConnected(advertisementFromDb.UserId);
			return viewModel;
		}

		private async Task<List<byte[]>> GetPhotosList(List<AdvertisementPhoto> photos)
		{
			var photosList = new List<byte[]>();
			foreach (var photo in photos)
			{
				photosList.Add(await this.advertisementItemPhotosService.GetPhotoInBytes(photo.PhotoPath));
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
				viewModel.AdvertisementTitle = dbModel.Title;
				viewModel.AdvertisementPrice = dbModel.Price;
				viewModel.MainPhoto = await this.advertisementItemPhotosService.GetPhotoInBytes(dbModel.AdvertisementPhotos.FirstOrDefault(p => p.IsMainPhoto).PhotoPath);
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
				viewModelsList.Add(viewModel);
			}

			return viewModelsList;
		}

		private List<AdvertisementPhoto> CreateAdvertisementPhotosModels(IEnumerable<string> advertisementPhotosPaths)
		{
			var photosDbModelsList = new List<AdvertisementPhoto>();
			foreach (var photoPath in advertisementPhotosPaths)
			{
				var model = new AdvertisementPhoto();
				model.PhotoPath = photoPath;
				var a = Path.GetDirectoryName(photoPath);
				model.IsMainPhoto = this.appFilesPathHelper.IsMiniaturePhotoDirectory(photoPath);
				photosDbModelsList.Add(model);
			}

			return photosDbModelsList;
		}

	}
}
