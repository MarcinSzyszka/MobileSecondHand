using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Models.Advertisement {
	public class NewAdvertisementItem {
		public string AdvertisementTitle { get; set; }
		public string AdvertisementDescription { get; set; }
		public int AdvertisementPrice { get; set; }
		public bool IsOnlyForSell { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public List<string> PhotosPaths { get; set; }
	}
}
