using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Db.Models.Advertisement.Keywords {
	public class CategoryKeyword : IKeyword {
		public int Id { get; set; }
		public string Name { get; set; }

		public virtual ICollection<CategoryKeywordToAdvertisement> Advertisements { get; set; }
	}
}
