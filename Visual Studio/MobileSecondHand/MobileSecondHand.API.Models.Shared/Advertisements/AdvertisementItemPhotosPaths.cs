using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
    public class AdvertisementItemPhotosNames
    {
		public List<string> PhotosNames { get; set; }

		public AdvertisementItemPhotosNames() {
			this.PhotosNames = new List<string>();
		}
	}
}
