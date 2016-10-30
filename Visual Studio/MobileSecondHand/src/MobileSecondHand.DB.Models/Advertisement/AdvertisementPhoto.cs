using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.DB.Models.Advertisement
{
    public class AdvertisementPhoto
    {
		public int AdvertisementPhotoId { get; set; }
		public string PhotoName { get; set; }
		public bool IsMainPhoto { get; set; }
		public int AdvertisementItemId { get; set; }
		[ForeignKey("AdvertisementItemId")]
		public AdvertisementItem AdvertisementItem { get; set; }
	}
}
