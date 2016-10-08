using System.Collections.Generic;
using MobileSecondHand.API.Models.Shared.Location;

namespace MobileSecondHand.Models.Settings
{
	public class AppSettingsModel
	{
		public CoordinatesForAdvertisementsModel LocationSettings { get; set; } = new CoordinatesForAdvertisementsModel();
		public IDictionary<int, string> Keywords { get; set; } = new Dictionary<int, string>();
		public bool ChatDisabled { get; set; }
		public bool NotificationsDisabled { get; set; }
	}
}
