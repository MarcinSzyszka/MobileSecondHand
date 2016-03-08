using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Location;

namespace MobileSecondHand.Api.Models.Advertisement
{
    public class NewAdvertisementItemModel
    {
		public string AdvertisementTitle { get; set; }
		public string AdvertisementDescription { get; set; }
		public int AdvertisementPrice { get; set; }
		public Coordinates AdvertisementCoordinates { get; set; }
		//public Dictionary<string, byte[]> AdvertisementPhotos { get; set; }
	}
}
