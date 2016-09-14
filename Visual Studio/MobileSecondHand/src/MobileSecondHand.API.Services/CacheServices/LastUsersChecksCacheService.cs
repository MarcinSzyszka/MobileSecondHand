using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using MobileSecondHand.API.Models.CheckingNewAdvertisements;

namespace MobileSecondHand.API.Services.CacheServices
{
	public class LastUsersChecksCacheService : ILastUsersChecksCacheService
	{

		MemoryCache cache;
		const string USERS_CHECKING_LIST = "Users_Checking_List";
		public LastUsersChecksCacheService()
		{
			this.cache = MemoryCache.Default;
		}

		public LastTimeUserCheckModel GetLastTimeUserCheck(string userId)
		{
			var usersCheckingList = GetUsersCheckingList();

			var userCheckModel = usersCheckingList.FirstOrDefault(u => u.UserId == userId);
			if (userCheckModel == null)
			{
				userCheckModel = new LastTimeUserCheckModel
				{
					UserId = userId
				};

				usersCheckingList.Add(userCheckModel);
			}

			return userCheckModel;
		}

		private List<LastTimeUserCheckModel> GetUsersCheckingList()
		{
			var cacheObject = this.cache.Get(USERS_CHECKING_LIST);
			if (cacheObject == null)
			{
				var userCheckingList = new List<LastTimeUserCheckModel>();
				this.cache.Set(USERS_CHECKING_LIST, userCheckingList, ObjectCache.InfiniteAbsoluteExpiration);
			}

			return (List<LastTimeUserCheckModel>)this.cache.Get(USERS_CHECKING_LIST);
		}

		public void UpdateLastTimeUserCheckDate(string userId, bool currentLocation)
		{
			var lastCheckModel = GetLastTimeUserCheck(userId);
			if (currentLocation)
			{
				lastCheckModel.LastAroundCurrentLocationCheckDate = DateTime.Now;
			}
			else
			{
				lastCheckModel.LastAroundHomeLocationCheckDate = DateTime.Now;
			}

		}
	}
}
