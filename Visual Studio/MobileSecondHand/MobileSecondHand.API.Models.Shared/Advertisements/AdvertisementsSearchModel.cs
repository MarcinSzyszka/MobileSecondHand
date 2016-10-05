using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
	public class AdvertisementsSearchModel
	{
		public IDictionary<int, string> CategoriesModel { get; set; } = new Dictionary<int, string>();
	}
}
