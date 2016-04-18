using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Db.Models.Advertisement.Keywords;

namespace MobileSecondHand.Db.Models.Advertisement
{
    public class CategoryKeywordToAdvertisement
    {
		public int CategoryKeywordId { get; set; }
		public CategoryKeyword CategoryKeyword { get; set; }

		public int AdvertisementItemId { get; set; }
		public AdvertisementItem AdvertisementItem { get; set; }
	}
}
