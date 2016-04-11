using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Api.Models.Coordinates;
using MobileSecondHand.Common.CoordinatesHelpers;
using MobileSecondHand.Common.PathHelpers;
using MobileSecondHand.Db.Models.Advertisement;
using MobileSecondHand.Db.Services.Advertisement;

namespace MobileSecondHand.Api.Services.Advertisement
{
    public class AdvertisementItemService : IAdvertisementItemService {
		IAdvertisementItemDbService advertisementItemDbService;
		ICoordinatesCalculator coordinatesCalculator;
		IAdvertisementItemPhotosService advertisementItemPhotosService;
		IAppFilesPathHelper appFilesPathHelper;

		public AdvertisementItemService(IAdvertisementItemDbService advertisementItemDbService, ICoordinatesCalculator coordinatesCalculator, IAdvertisementItemPhotosService advertisementItemPhotosService, IAppFilesPathHelper appFilesPathHelper) {
			this.advertisementItemDbService = advertisementItemDbService;
			this.coordinatesCalculator = coordinatesCalculator;
			this.advertisementItemPhotosService = advertisementItemPhotosService;
			this.appFilesPathHelper = appFilesPathHelper;
		}

		public void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel, string userId) {
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

			this.advertisementItemDbService.SaveNewAdvertisementItem(model);
		}

		public async Task<IEnumerable<AdvertisementItemShortModel>> GetAdvertisements(SearchModel searchModel, string userId) {
			var coordinatesForSearchModel = coordinatesCalculator.GetCoordinatesForSearchingAdvertisements(searchModel.CoordinatesModel.Latitude, searchModel.CoordinatesModel.Longitude, searchModel.CoordinatesModel.MaxDistance);
			var advertisementsFromDb = this.advertisementItemDbService.GetAdvertisementsFromDeclaredArea(coordinatesForSearchModel, searchModel.Page);
			IEnumerable<AdvertisementItemShortModel> advertisementsViewModels = await MapDbModelsToShortViewModels(advertisementsFromDb, searchModel.CoordinatesModel);

			return advertisementsViewModels;
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
