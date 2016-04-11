using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Location;

namespace MobileSecondHand.Models.Advertisement {
	public class SearchModel {
		public CoordinatesForAdvertisementsModel CoordinatesModel { get; set; }
		public int Page { get; set; }
	}
}
