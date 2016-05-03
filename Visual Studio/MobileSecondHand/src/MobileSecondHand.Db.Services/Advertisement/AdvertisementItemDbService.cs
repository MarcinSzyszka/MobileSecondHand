using System;
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

		public AdvertisementItem GetAdvertisementDetails(int advertisementId) {
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos)
												.Include(a => a.CategoryKeywords).ThenInclude(a => a.CategoryKeyword)
												.Include(a => a.ColorKeywords).ThenInclude(a => a.ColorKeyword)
												.FirstOrDefault(a => a.Id == advertisementId);
		}

		public IEnumerable<AdvertisementItem> GetAdvertisementsFromDeclaredArea(CoordinatesForSearchingAdvertisementsModel coordinatesForSearchModel, int page) {
			//dorobić pejdzowanie
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.Latitude >= coordinatesForSearchModel.LatitudeStart
																						&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
																						&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
																						&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd)
																				  .Skip(10 * page).Take(10);
		}

		public void SaveNewAdvertisementItem(AdvertisementItem advertisementItem) {
			dbContext.AdvertisementItem.Add(advertisementItem);
			dbContext.SaveChanges();
		}
	}
}
