using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Models.Advertisement {
	public class AdvertisementItemPhotosPaths {
		public List<string> PhotosPaths { get; set; }

		public AdvertisementItemPhotosPaths() {
			this.PhotosPaths = new List<string>();
		}
	}
}
