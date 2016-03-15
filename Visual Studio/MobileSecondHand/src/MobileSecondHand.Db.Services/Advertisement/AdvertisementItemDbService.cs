using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using MobileSecondHand.Common.Models;
using MobileSecondHand.Db.Models.Advertisement;

namespace MobileSecondHand.Db.Services.Advertisement {
	public class AdvertisementItemDbService : IAdvertisementItemDbService {
		MobileSecondHandContext dbContext;
		public AdvertisementItemDbService(MobileSecondHandContext context) {
			this.dbContext = context;
		}

		public IEnumerable<AdvertisementItem> GetAdvertisementsFromDeclaredArea(CoordinatesForSearchingAdvertisementsModel coordinatesForSearchModel) {
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.Latitude >= coordinatesForSearchModel.LatitudeStart
																						&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
																						&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
																						&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd);
		}

		public void SaveNewAdvertisementItem(AdvertisementItem advertisementItem) {
			dbContext.AdvertisementItem.Add(advertisementItem);
			dbContext.SaveChanges();
		}
	}
}
