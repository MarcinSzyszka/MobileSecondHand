using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Db.Models.Advertisement.Keywords;

namespace MobileSecondHand.Db.Models.Advertisement
{
    public class ColorKeywordToAdvertisement
    {
		public int ColorKeywordId { get; set; }
		public ColorKeyword ColorKeyword { get; set; }

		public int AdvertisementItemId { get; set; }
		public AdvertisementItem AdvertisementItem { get; set; }
	}
}
