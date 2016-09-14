using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Advertisement;
using MobileSecondHand.API.Models.Coordinates;
using MobileSecondHand.API.Services.Keywords;
using MobileSecondHand.API.Services.CacheServices;
using MobileSecondHand.COMMON.CoordinatesHelpers;
using MobileSecondHand.COMMON.PathHelpers;
using MobileSecondHand.DB.Models.Advertisement;
using MobileSecondHand.DB.Services.Advertisement;
using MobileSecondHand.DB.Models.Keywords;

namespace MobileSecondHand.API.Services.Advertisement
{
	public class AdvertisementItemService : IAdvertisementItemService {
		IAdvertisementItemDbService advertisementItemDbService;
		ICoordinatesCalculator coordinatesCalculator;
		IAdvertisementItemPhotosService advertisementItemPhotosService;
		IAppFilesPathHelper appFilesPathHelper;
		IKeywordsService keywordsService;
		IChatHubCacheService chatHubCacheService;
		ILastUsersChecksCacheService lastUsersChecksCacheService;

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

		public void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel, string userId) {
			var advertisementItemDescription = String.Concat(newAdvertisementModel.AdvertisementTitle, ' ', newAdvertisementModel.AdvertisementDescription);
			var categoryKeywords = this.keywordsService.RecognizeAndGetKeywordsDbModels<CategoryKeyword>(advertisementItemDescription);
			var colorKeywords = this.keywordsService.RecognizeAndGetKeywordsDbModels<ColorKeyword>(advertisementItemDescription);

			var model = new AdvertisementItem {
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
				AdvertisementPhotos = CreateAdvertisementPhotosModels(newAdvertisementModel.PhotosPaths)
			};

			foreach (var category in categoryKeywords) {
				model.CategoryKeywords.Add(new CategoryKeywordToAdvertisement { AdvertisementItem = model, CategoryKeyword = category });
			}

			foreach (var color in colorKeywords) {
				model.ColorKeywords.Add(new ColorKeywordToAdvertisement { AdvertisementItem = model, ColorKeyword = color });
			}


			this.advertisementItemDbService.SaveNewAdvertisementItem(model);
		}

		public async Task<IEnumerable<AdvertisementItemShortModel>> GetAdvertisements(SearchAdvertisementsModel searchModel, string userId) {
			var coordinatesForSearchModel = coordinatesCalculator.GetCoordinatesForSearchingAdvertisements(searchModel.CoordinatesModel.Latitude, searchModel.CoordinatesModel.Longitude, searchModel.CoordinatesModel.MaxDistance);
			var advertisementsFromDb = this.advertisementItemDbService.GetAdvertisementsFromDeclaredArea(coordinatesForSearchModel, searchModel.Page).ToList();
			IEnumerable<AdvertisementItemShortModel> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb, searchModel.CoordinatesModel);

			return advertisementsViewModels;
		}

		public async Task<IEnumerable<AdvertisementItemShortModel>> GetUserAdvertisements(string userId, int pageNumber)
		{
			var advertisementsFromDb = this.advertisementItemDbService.GetUserAdvertisements(userId, pageNumber).ToList();
			IEnumerable<AdvertisementItemShortModel> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb);

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

		public async Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementId, string userId) {
			var advertisementDetailsViewModel = default(AdvertisementItemDetails);
			var advertisementFromDb = this.advertisementItemDbService.GetByIdWithDetails(advertisementId);
			if (advertisementFromDb != null) {
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

		private async Task<AdvertisementItemDetails> MapToDetailsViewModel(AdvertisementItem advertisementFromDb) {
			var viewModel = new AdvertisementItemDetails();
			viewModel.Id = advertisementFromDb.Id;
			viewModel.Title = advertisementFromDb.Title;
			viewModel.Description = advertisementFromDb.Description;
			viewModel.Price = advertisementFromDb.Price;
			viewModel.IsOnlyForSell = advertisementFromDb.IsOnlyForSell;
			viewModel.SellerId = advertisementFromDb.UserId;
			viewModel.Photo = await this.advertisementItemPhotosService.GetPhotoInBytes(advertisementFromDb.AdvertisementPhotos.FirstOrDefault(p => !p.IsMainPhoto).PhotoPath);
			viewModel.IsSellerOnline = this.chatHubCacheService.IsUserConnected(advertisementFromDb.UserId);
			return viewModel;
		}

		private async Task<IEnumerable<AdvertisementItemShortModel>> MapDbModelsToShortViewModels(IEnumerable<AdvertisementItem> advertisementsFromDb, CoordinatesForAdvertisementsModel coordinatesModel = null) {
			var viewModelsList = new List<AdvertisementItemShortModel>();
			foreach (var dbModel in advertisementsFromDb) {
				var viewModel = new AdvertisementItemShortModel();
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

		private List<AdvertisementPhoto> CreateAdvertisementPhotosModels(IEnumerable<string> advertisementPhotosPaths) {
			var photosDbModelsList = new List<AdvertisementPhoto>();
			foreach (var photoPath in advertisementPhotosPaths) {
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
