using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Models.Keywords {
	public class CategoryKeyword : IKeywordDbModel {
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }

		public List<CategoryKeywordToAdvertisement> Advertisements { get; set; } = new List<CategoryKeywordToAdvertisement>();
	}
}
