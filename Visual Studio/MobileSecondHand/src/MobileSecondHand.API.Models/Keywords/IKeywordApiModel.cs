using System.Collections.Generic;
using MobileSecondHand.API.Models.Shared.Advertisements;

namespace MobileSecondHand.API.Models.Keywords
{
	public interface IKeywordApiModel {
		int Id { get; set; }
		string Name { get; set; }
		IEnumerable<AdvertisementItemShort> AdvertisementsShortModels { get; set; }
	}
}
