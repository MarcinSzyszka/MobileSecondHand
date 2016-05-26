using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Advertisement.Keywords;

namespace MobileSecondHand.DB.Models.Advertisement
{
    public class CategoryKeywordToAdvertisement
    {
		public int CategoryKeywordId { get; set; }
		public CategoryKeyword CategoryKeyword { get; set; }

		public int AdvertisementItemId { get; set; }
		public AdvertisementItem AdvertisementItem { get; set; }
	}
}
