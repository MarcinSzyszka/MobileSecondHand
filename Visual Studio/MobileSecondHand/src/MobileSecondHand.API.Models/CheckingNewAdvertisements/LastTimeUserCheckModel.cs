using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.CheckingNewAdvertisements
{
	public class LastTimeUserCheckModel
	{
		public string UserId { get; set; }
		public DateTime LastAroundCurrentLocationCheckDate { get; set; } = DateTime.Now;
		public DateTime LastAroundHomeLocationCheckDate { get; set; } = DateTime.Now;
	}
}
