using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Enumerations
{
	public enum SortingBy
	{
		[DisplayName("Wg odległości (od najmniejszej)")]
		sortByNearest = 1,
		[DisplayName("Wg odległości (od największej)")]
		sortByFarthest = 2,
		[DisplayName("Wg ceny (od najniższej)")]
		sortByLowestPrice = 3,
		[DisplayName("Wg ceny (od najwyższej)")]
		sortByHighestPrice = 4,
		[DisplayName("Wg daty dodania (od najnowszych)")]
		sortByNewest = 5
	}
}
