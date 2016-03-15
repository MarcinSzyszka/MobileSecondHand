using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Common.Models
{
    public class CoordinatesForSearchingAdvertisementsModel
    {
		public double LatitudeStart { get; set; }
		public double LatitudeEnd { get; set; }
		public double LongitudeStart { get; set; }
		public double LongitudeEnd { get; set; }
	}
}
