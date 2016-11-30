using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Categories;
using MobileSecondHand.API.Models.Shared.Enumerations;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
	public class AdvertisementItemDetails
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public ClothSize Size { get; set; }
		public CategoryInfoModel CategoryInfoModel { get; set; }
		public int Price { get; set; }
		public List<byte[]> Photos { get; set; }
		public bool IsOnlyForSell { get; set; }
		public bool IsSellerOnline { get; set; }
		public string SellerId { get; set; }
		public string SellerName { get; set; }
		public bool IsActive { get; set; }
		public DateTime ExpirationDate { get; set; }
		public byte[] SellerProfileImage { get; set; }
	}
}
