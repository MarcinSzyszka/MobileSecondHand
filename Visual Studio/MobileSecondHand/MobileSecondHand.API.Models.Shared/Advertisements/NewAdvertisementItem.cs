using System.Collections.Generic;
using MobileSecondHand.API.Models.Shared.Categories;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
	public class NewAdvertisementItem
    {
		public string AdvertisementTitle { get; set; }
		public string AdvertisementDescription { get; set; }
		public int AdvertisementPrice { get; set; }
		public bool IsOnlyForSell { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public CategoryInfoModel Category { get; set; }
		public List<string> PhotosPaths { get; set; }
	}
}
