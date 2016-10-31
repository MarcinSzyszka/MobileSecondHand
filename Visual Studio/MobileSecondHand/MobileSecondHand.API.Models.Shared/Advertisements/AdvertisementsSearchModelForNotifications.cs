using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
	public class AdvertisementsSearchModelForNotifications
	{
		public List<ClothSize> Sizes { get; set; }
		public List<int> CategoriesIds { get; set; }
		public CoordinatesForAdvertisementsModel CoordinatesModels { get; set; } = new CoordinatesForAdvertisementsModel();
	}
}
