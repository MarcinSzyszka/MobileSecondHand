using MobileSecondHand.API.Models.Shared.Location;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
	public class SearchAdvertisementsModel
	{
		public CoordinatesForAdvertisementsModel CoordinatesModel { get; set; } = new CoordinatesForAdvertisementsModel();
		public int Page { get; set; }
	}
}
