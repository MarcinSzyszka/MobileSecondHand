using System.Collections.Generic;
using MobileSecondHand.API.Models.Shared.Advertisements;

namespace MobileSecondHand.API.Models.Keywords
{
	public class CategoryKeywordApiModel : IKeywordApiModel {
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<AdvertisementItemShort> AdvertisementsShortModels { get; set; }
	}
}
