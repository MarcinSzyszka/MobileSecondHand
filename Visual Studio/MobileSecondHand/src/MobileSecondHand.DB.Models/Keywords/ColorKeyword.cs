using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Models.Keywords {
	public class ColorKeyword : IKeywordDbModel {
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }

		public List<ColorKeywordToAdvertisement> Advertisements { get; set; } = new List<ColorKeywordToAdvertisement>();
	}
}
