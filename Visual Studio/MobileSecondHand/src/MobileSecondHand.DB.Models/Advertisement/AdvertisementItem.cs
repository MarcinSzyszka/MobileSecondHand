using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.DB.Models.Advertisement
{
	public class AdvertisementItem
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Price { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public bool IsActive { get; set; }
		public bool IsOnlyForSell { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime? ExpirationDate { get; set; }
		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
		public List<AdvertisementPhoto> AdvertisementPhotos { get; set; } = new List<AdvertisementPhoto>();
		public List<ColorKeywordToAdvertisement> ColorKeywords { get; set; } = new List<ColorKeywordToAdvertisement>();
		public List<CategoryKeywordToAdvertisement> CategoryKeywords { get; set; } = new List<CategoryKeywordToAdvertisement>();
	}
}
