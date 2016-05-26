using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Advertisement.Keywords {
	public interface IKeywordApiModel {
		int Id { get; set; }
		string Name { get; set; }
		IEnumerable<AdvertisementItemShortModel> AdvertisementsShortModels { get; set; }
	}
}
