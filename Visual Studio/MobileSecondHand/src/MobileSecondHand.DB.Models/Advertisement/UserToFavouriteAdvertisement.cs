using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.DB.Models.Advertisement
{
	public class UserToFavouriteAdvertisement
	{
		public string ApplicationUserId { get; set; }
		public ApplicationUser ApplicationUser { get; set; }

		public int AdvertisementItemId { get; set; }
		public AdvertisementItem AdvertisementItem { get; set; }
	}
}
