using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Db.Models.Advertisement;

namespace MobileSecondHand.Db.Services.Advertisement
{
    public interface IAdvertisementItemDbService
    {
		void SaveNewAdvertisementItem(AdvertisementItem advertisementItem);

	}
}
