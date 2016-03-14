using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Api.Models.Coordinates
{
    public class CoordinatesModel
    {
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int MaxDistance { get; set; }
	}
}
