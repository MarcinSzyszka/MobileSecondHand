using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Advertisement;
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

		private ICollection<AdvertisementPhoto> CreateAdvertisementPhotosModels(IEnumerable<string> advertisementPhotosPaths) {
			var photosDbModelsList = new List<AdvertisementPhoto>();
			foreach (var photoPath in advertisementPhotosPaths) {
				photosDbModelsList.Add(new AdvertisementPhoto {
					PhotoPath = photoPath
				});
			}

			return photosDbModelsList;
		}
	}
}
