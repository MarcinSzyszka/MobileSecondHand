using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Categories;
using MobileSecondHand.API.Models.Shared.Enumerations;

namespace MobileSecondHand.Models.Advertisement
{
	public class AdvertisementEditModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public ClothSize Size { get; set; }
		public CategoryInfoModel CategoryInfoModel { get; set; }
		public int Price { get; set; }
		public bool IsOnlyForSell { get; set; }
		public List<string> PhotosPaths { get; set; }
	}
}
