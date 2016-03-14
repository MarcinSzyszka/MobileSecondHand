using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Api.Models.Coordinates;
using MobileSecondHand.Common.PathHelpers;
using MobileSecondHand.Db.Models.Advertisement;
using MobileSecondHand.Db.Services.Advertisement;

namespace MobileSecondHand.Api.Services.Advertisement
{
    public class AdvertisementItemService : IAdvertisementItemService {
		IAdvertisementItemDbService advertisementItemDbService;

		public AdvertisementItemService(IAdvertisementItemDbService advertisementItemDbService) {
			this.advertisementItemDbService = advertisementItemDbService;
		}

		public void CreateNewAdvertisementItem(NewAdvertisementItemModel newAdvertisementModel) {
			var model = new AdvertisementItem {
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

		public IEnumerable<AdvertisementItemShortModel> GetAdvertisements(CoordinatesModel coordinatesModel) {
			//pobieranko z bazki
			for (int i = 0; i < 10; i++) {
				yield return new AdvertisementItemShortModel { AdvertisementTitle = "lalalal", AdvertisementPrice = 5 };
			}
		}

		private ICollection<AdvertisementPhoto> CreateAdvertisementPhotosModels(IEnumerable<string> advertisementPhotosPaths) {
			var photosDbModelsList = new List<AdvertisementPhoto>();
			foreach (var photoPath in advertisementPhotosPaths) {
				var model = new AdvertisementPhoto();
				model.PhotoPath = photoPath;
				model.IsMainPhoto = Path.GetDirectoryName(photoPath) == AppFilesPathHelper.MIN_PHOTOS_DIRECTORY_NAME;
				photosDbModelsList.Add(model);
			}

			return photosDbModelsList;
		}
	}
}
