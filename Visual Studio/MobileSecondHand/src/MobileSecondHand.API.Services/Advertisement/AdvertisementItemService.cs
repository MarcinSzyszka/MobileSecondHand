using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Advertisement;
using MobileSecondHand.API.Models.Coordinates;
using MobileSecondHand.API.Services.Advertisement.Keywords;
using MobileSecondHand.API.Services.CacheServices;
using MobileSecondHand.COMMON.CoordinatesHelpers;
using MobileSecondHand.COMMON.PathHelpers;
using MobileSecondHand.DB.Models.Advertisement;
using MobileSecondHand.DB.Models.Advertisement.Keywords;
using MobileSecondHand.DB.Services.Advertisement;

namespace MobileSecondHand.API.Services.Advertisement {
	public class AdvertisementItemService : IAdvertisementItemService {
		IAdvertisementItemDbService advertisementItemDbService;
		ICoordinatesCalculator coordinatesCalculator;
		IAdvertisementItemPhotosService advertisementItemPhotosService;
		IAppFilesPathHelper appFilesPathHelper;
		IKeywordsService keywordsService;
		IChatHubCacheService chatHubCacheService;

		public AdvertisementItemService(IAdvertisementItemDbService advertisementItemDbService, ICoordinatesCalculator coordinatesCalculator, IAdvertisementItemPhotosService advertisementItemPhotosService, IAppFilesPathHelper appFilesPathHelper, IKeywordsService keywordsService, IChatHubCacheService chatHubCacheService)
		{
			this.advertisementItemDbService = advertisementItemDbService;
			this.coordinatesCalculator = coordinatesCalculator;
			this.advertisementItemPhotosService = advertisementItemPhotosService;
			this.appFilesPathHelper = appFilesPathHelper;
			this.keywordsService = keywordsService;
			this.chatHubCacheService = chatHubCacheService;
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

		public async Task<IEnumerable<AdvertisementItemShortModel>> GetAdvertisements(SearchModel searchModel, string userId){
			var coordinatesForSearchModel = coordinatesCalculator.GetCoordinatesForSearchingAdvertisements(searchModel.CoordinatesModel.Latitude, searchModel.CoordinatesModel.Longitude, searchModel.CoordinatesModel.MaxDistance);
			var advertisementsFromDb = this.advertisementItemDbService.GetAdvertisementsFromDeclaredArea(coordinatesForSearchModel, searchModel.Page);
			IEnumerable<AdvertisementItemShortModel> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb, searchModel.CoordinatesModel);

			return advertisementsViewModels;
		}

		public async Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementId, string userId) {
			var advertisementDetailsViewModel = default(AdvertisementItemDetails);
			var advertisementFromDb = this.advertisementItemDbService.GetAdvertisementDetails(advertisementId);
			if (advertisementFromDb != null) {
				advertisementDetailsViewModel = await MapToDetailsViewModel(advertisementFromDb);
			}

			return advertisementDetailsViewModel;
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

		private async Task<IEnumerable<AdvertisementItemShortModel>> MapDbModelsToShortViewModels(IEnumerable<AdvertisementItem> advertisementsFromDb, CoordinatesForAdvertisementsModel coordinatesModel) {
			var viewModelsList = new List<AdvertisementItemShortModel>();
			foreach (var dbModel in advertisementsFromDb) {
				var viewModel = new AdvertisementItemShortModel();
				viewModel.Id = dbModel.Id;
				viewModel.AdvertisementTitle = dbModel.Title;
				viewModel.AdvertisementPrice = dbModel.Price;
				viewModel.MainPhoto = await this.advertisementItemPhotosService.GetPhotoInBytes(dbModel.AdvertisementPhotos.FirstOrDefault(p => p.IsMainPhoto).PhotoPath);
				viewModel.Distance = this.coordinatesCalculator.GetDistanceBetweenTwoLocalizations(coordinatesModel.Latitude, coordinatesModel.Longitude, dbModel.Latitude, dbModel.Longitude);

				viewModelsList.Add(viewModel);
			}

			return viewModelsList;
		}

		private ICollection<AdvertisementPhoto> CreateAdvertisementPhotosModels(IEnumerable<string> advertisementPhotosPaths) {
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
