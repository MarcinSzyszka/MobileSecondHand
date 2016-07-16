using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Advertisement;

namespace MobileSecondHand.API.Models.Keywords
{
	public class CategoryKeywordApiModel : IKeywordApiModel {
		public int Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<AdvertisementItemShortModel> AdvertisementsShortModels { get; set; }
	}
}
