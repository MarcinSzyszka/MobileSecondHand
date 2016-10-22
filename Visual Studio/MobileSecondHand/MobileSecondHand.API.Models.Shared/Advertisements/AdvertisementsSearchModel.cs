using System.Collections.Generic;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
	public class AdvertisementsSearchModel
	{
		public IDictionary<int, string> CategoriesModel { get; set; } = new Dictionary<int, string>();
		public AdvertisementsKind AdvertisementsKind { get; set; }
		public CoordinatesForAdvertisementsModel CoordinatesModel { get; set; } = new CoordinatesForAdvertisementsModel();
		public int Page { get; set; }
		public UserInfoModel UserInfo { get; set; }
	}
}
