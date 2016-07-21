using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MobileSecondHand.COMMON.Models;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Services.Advertisement {
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
			//return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.Latitude >= coordinatesForSearchModel.LatitudeStart
			//																			&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
			//																			&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
			//																			&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd)
			//																	  .Skip(10 * page).Take(10);

			//workaround buga w EF (chujowe bo ciagne wszystko zeby zwrocic 10)
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.Latitude >= coordinatesForSearchModel.LatitudeStart
																						&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
																						&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
																						&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd)
																						.ToList()
																						.Skip(10 * page)
																						.Take(10);

		}

		public void SaveNewAdvertisementItem(AdvertisementItem advertisementItem) {
			dbContext.AdvertisementItem.Add(advertisementItem);
			dbContext.Entry(advertisementItem).State = EntityState.Added;
			foreach (var keyword in advertisementItem.CategoryKeywords)
			{
				if (keyword.CategoryKeyword.Id > 0)
				{
					dbContext.Entry(keyword.CategoryKeyword).State = EntityState.Unchanged;
				}
			}
			foreach (var keyword in advertisementItem.ColorKeywords)
			{
				if (keyword.ColorKeyword.Id > 0)
				{
					dbContext.Entry(keyword.ColorKeyword).State = EntityState.Unchanged;
				}
			}
			dbContext.SaveChanges();
		}
	}
}
