using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MobileSecondHand.COMMON.CoordinatesHelpers;
using MobileSecondHand.COMMON.Models;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Services.Advertisement
{
	public class AdvertisementItemDbService : IAdvertisementItemDbService
	{
		MobileSecondHandContext dbContext;
		ICoordinatesCalculator coordinatesCalculator;
		const int ITEMS_PER_REQUEST = 20;

		public AdvertisementItemDbService(MobileSecondHandContext context, ICoordinatesCalculator coordinatesCalculator)
		{
			this.dbContext = context;
			this.coordinatesCalculator = coordinatesCalculator;
		}

		public AdvertisementItem GetAdvertisementDetails(int advertisementId)
		{
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos)
												.Include(a => a.CategoryKeywords).ThenInclude(a => a.CategoryKeyword)
												.Include(a => a.ColorKeywords).ThenInclude(a => a.ColorKeyword)
												.FirstOrDefault(a => a.Id == advertisementId);
		}

		public IEnumerable<AdvertisementItem> GetAdvertisementsFromDeclaredArea(CoordinatesForSearchingAdvertisementsModel coordinatesForSearchModel, int page)
		{
			//return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.ExpirationDate >= DateTime.Now &&
			//																			a.Latitude >= coordinatesForSearchModel.LatitudeStart
			//																			&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
			//																			&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
			//																			&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd)
			//																			.OrderBy(a => a.CreationDate)
			//																		    .Skip(10 * page).Take(10);

			//workaround buga w EF (chujowe bo ciagne wszystko zeby zwrocic 10)
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.ExpirationDate >= DateTime.Now &&
																						a.Latitude >= coordinatesForSearchModel.LatitudeStart
																						&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
																						&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
																						&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd)
																						.OrderBy(a => this.coordinatesCalculator.GetDistanceBetweenTwoLocalizations(coordinatesForSearchModel.UserLatitude, coordinatesForSearchModel.UserLongitude, a.Latitude, a.Longitude))
																						.ToList()
																						.Skip(ITEMS_PER_REQUEST * page)
																						.Take(ITEMS_PER_REQUEST);

		}

		public IEnumerable<AdvertisementItem> GetUserAdvertisements(string userId, int pageNumber)
		{
			//	return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.UserId == userId)
			//																			.OrderBy(a => a.CreationDate)
			//																			.Skip(pageNumber * ITEMS_PER_REQUEST)
			//



			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.UserId == userId 
																						&& a.ExpirationDate >= DateTime.Now)
																						.OrderBy(a => a.CreationDate).ToList()
																						.Skip(pageNumber * ITEMS_PER_REQUEST)
																						.Take(ITEMS_PER_REQUEST);
		}

		public void SaveNewAdvertisementItem(AdvertisementItem advertisementItem)
		{
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
