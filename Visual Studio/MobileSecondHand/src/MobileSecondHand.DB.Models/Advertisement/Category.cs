using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.DB.Models.Advertisement
{
	/// <summary>
	/// Model reprezentuje kategorię ogłoszenia zdefinowaną przez użytkownika dodającego ogłoszenie
	/// </summary>
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<AdvertisementItem> Advertisements { get; set; } = new List<AdvertisementItem>();
	}
}
