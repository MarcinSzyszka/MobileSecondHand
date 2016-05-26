using MobileSecondHand.COMMON.Models;

namespace MobileSecondHand.COMMON.CoordinatesHelpers {
	public interface ICoordinatesCalculator {
		CoordinatesForSearchingAdvertisementsModel GetCoordinatesForSearchingAdvertisements(double lat, double lon, int distanceInKm);
		double GetDistanceBetweenTwoLocalizations(double latitude1, double longitude1, double latitude2, double longitude2);
	}
}