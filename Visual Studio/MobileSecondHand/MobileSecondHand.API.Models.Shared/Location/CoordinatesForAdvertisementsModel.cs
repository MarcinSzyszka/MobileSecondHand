using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Location {
	public class CoordinatesForAdvertisementsModel {
		public double Latitude { get; set; }
		/// <summary>
		/// Only for home location setting
		/// </summary>
		public string LocationAddress { get; set; }
		public double Longitude { get; set; }
		public int MaxDistance { get; set; }
	}
}
