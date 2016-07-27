using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.CheckingNewAdvertisements;

namespace MobileSecondHand.API.Services.CacheServices
{
	public interface ILastUsersChecksCacheService
	{
		LastTimeUserCheckModel GetLastTimeUserCheck(string userId);
		void UpdateLastTimeUserCheckDate(string userId);
	}
}
