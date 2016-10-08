using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
    public class AdvertisementItemPhotosPaths
    {
		public List<string> PhotosPaths { get; set; }

		public AdvertisementItemPhotosPaths() {
			this.PhotosPaths = new List<string>();
		}
	}
}
