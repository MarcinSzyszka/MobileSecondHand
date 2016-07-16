using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Location;

namespace MobileSecondHand.Models.Settings
{
	public class AppSettingsModel
	{
		public CoordinatesForAdvertisementsModel LocationSettings { get; set; } = new CoordinatesForAdvertisementsModel();
		public IDictionary<int, string> Keywords { get; set; } = new Dictionary<int, string>();
	}
}
