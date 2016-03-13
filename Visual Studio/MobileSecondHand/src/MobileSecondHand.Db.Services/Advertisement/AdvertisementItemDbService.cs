using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Db.Models.Advertisement;

namespace MobileSecondHand.Db.Services.Advertisement
{
    public class AdvertisementItemDbService : IAdvertisementItemDbService {
		MobileSecondHandContext dbContext;
		public AdvertisementItemDbService(MobileSecondHandContext context) {
			this.dbContext = context;
		}

		public void SaveNewAdvertisementItem(AdvertisementItem advertisementItem) {
			dbContext.AdvertisementItem.Add(advertisementItem);
			dbContext.SaveChanges();
		}
	}
}
