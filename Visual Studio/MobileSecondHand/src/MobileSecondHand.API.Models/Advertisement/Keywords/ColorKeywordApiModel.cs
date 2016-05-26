using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Advertisement.Keywords
{
	public class ColorKeywordApiModel : IKeywordApiModel {
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<AdvertisementItemShortModel> AdvertisementsShortModels {get; set;}
	}
}
