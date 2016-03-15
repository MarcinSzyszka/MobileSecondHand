using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Common.Models;

namespace MobileSecondHand.Common.CoordinatesHelpers
{
    public class CoordinatesCalculator : ICoordinatesCalculator {
		const double DEGRESS_PER_KILOMETER = 0.0111;
		public CoordinatesForSearchingAdvertisementsModel GetCoordinatesForSearchingAdvertisements(double lat, double lon, int distanceInKm) {
			double radius = ((double)distanceInKm) / 2;
			var model = new CoordinatesForSearchingAdvertisementsModel();
			model.LatitudeStart = lat - (radius * DEGRESS_PER_KILOMETER);
			model.LatitudeEnd = lat + (radius * DEGRESS_PER_KILOMETER);
			model.LongitudeStart = lon - (radius * DEGRESS_PER_KILOMETER);
			model.LongitudeEnd = lon + (radius * DEGRESS_PER_KILOMETER);

			return model;
		}

		public double GetDistanceBetweenTwoLocalizations(double latitude1, double longitude1, double latitude2, double longitude2) {
			var firstLocation = new GeoCoordinate(latitude1, longitude1);
			var secondLocation = new GeoCoordinate(latitude2, longitude2);
			var distance = Math.Round(firstLocation.GetDistanceTo(secondLocation), 2);

			return distance;
		}
	}
}
