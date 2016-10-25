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


		public AdvertisementItemDbService(MobileSecondHandContext context)
		{
			this.dbContext = context;
		}

		public AdvertisementItem GetByIdWithDetails(int advertisementId)
		{
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos)
												.Include(a => a.User)
												.FirstOrDefault(a => a.Id == advertisementId);
		}

		public IQueryable<AdvertisementItem> GetAdvertisements()
		{
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.ExpirationDate >= DateTime.Now);
		}

		public IEnumerable<AdvertisementItem> GetAdvertisementsFromDeclaredAreaSinceLastCheck(DateTime lastCheckDate, string userId, CoordinatesForSearchingAdvertisementsModel coordinatesForSearchModel)
		{
			return dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.CreationDate >= lastCheckDate && a.UserId != userId &&
																				a.Latitude >= coordinatesForSearchModel.LatitudeStart
																				&& a.Latitude <= coordinatesForSearchModel.LatitudeEnd
																				&& a.Longitude >= coordinatesForSearchModel.LongitudeStart
																				&& a.Longitude <= coordinatesForSearchModel.LongitudeEnd)
																				.Take(1);
		}

		public IQueryable<AdvertisementItem> GetUserAdvertisements(string userId, int pageNumber = -1)
		{
			IQueryable<AdvertisementItem> queryAdvertisements = dbContext.AdvertisementItem.Include(a => a.AdvertisementPhotos).Where(a => a.UserId == userId && a.ExpirationDate >= DateTime.Now).OrderBy(a => a.ExpirationDate);

			if (pageNumber > -1)
			{
				queryAdvertisements = queryAdvertisements.Skip(20 * pageNumber).Take(20);
			}


			return queryAdvertisements;
		}

		public UserToFavouriteAdvertisement GetUserFavouriteAdvertisement(string userId, int advertisementId)
		{
			return this.dbContext.UserToFavouriteAdvertisement.Include(f => f.AdvertisementItem).FirstOrDefault(f => f.ApplicationUserId == userId && f.AdvertisementItemId == advertisementId && f.AdvertisementItem.ExpirationDate >= DateTime.Now);
		}

		public IEnumerable<UserToFavouriteAdvertisement> GetUserFavouritesAdvertisements(string userId)
		{
			return this.dbContext.UserToFavouriteAdvertisement.Include(f => f.AdvertisementItem).ThenInclude(a => a.AdvertisementPhotos).Where(f => f.ApplicationUserId == userId && f.AdvertisementItem.ExpirationDate >= DateTime.Now);

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

		public AdvertisementItem GetById(int advertisementId)
		{
			return this.dbContext.AdvertisementItem.FirstOrDefault(a => a.Id == advertisementId);
		}

		public void SaveAdvertisementItem(AdvertisementItem advertisement)
		{
			this.dbContext.Entry(advertisement).State = EntityState.Modified;

			this.dbContext.SaveChanges();
		}

		public void SaveUserFavouriteAdvertisement(UserToFavouriteAdvertisement favouriteAdvertisement)
		{
			if (favouriteAdvertisement.ApplicationUser != null)
			{
				this.dbContext.Entry(favouriteAdvertisement).State = EntityState.Modified;
			}
			else
			{
				this.dbContext.UserToFavouriteAdvertisement.Add(favouriteAdvertisement);
				this.dbContext.Entry(favouriteAdvertisement).State = EntityState.Added;
			}

			this.dbContext.SaveChanges();
		}

		public void DeleteFavouriteAdvertisement(UserToFavouriteAdvertisement advertisement)
		{
			this.dbContext.UserToFavouriteAdvertisement.Remove(advertisement);
			this.dbContext.Entry(advertisement).State = EntityState.Deleted;

			this.dbContext.SaveChanges();
		}
	}
}
