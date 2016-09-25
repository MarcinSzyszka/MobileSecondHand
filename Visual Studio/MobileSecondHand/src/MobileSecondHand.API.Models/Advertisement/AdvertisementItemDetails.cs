using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Advertisement
{
	public class AdvertisementItemDetails
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Price { get; set; }
		public List<byte[]> Photos { get; set; }
		public bool IsOnlyForSell { get; set; }
		public bool IsSellerOnline { get; set; }
		public string SellerId { get; set; }
	}
}
