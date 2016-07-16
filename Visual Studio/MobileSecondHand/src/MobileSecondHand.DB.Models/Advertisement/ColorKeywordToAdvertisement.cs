using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Keywords;

namespace MobileSecondHand.DB.Models.Advertisement
{
    public class ColorKeywordToAdvertisement
    {
		public int ColorKeywordId { get; set; }
		public ColorKeyword ColorKeyword { get; set; }

		public int AdvertisementItemId { get; set; }
		public AdvertisementItem AdvertisementItem { get; set; }
	}
}
