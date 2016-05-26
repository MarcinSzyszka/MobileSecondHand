using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.DB.Models.Advertisement
{
    public class AdvertisementPhoto
    {
		public int Id { get; set; }
		public string PhotoPath { get; set; }
		public bool IsMainPhoto { get; set; }
		public int AdvertisementItemId { get; set; }
		[ForeignKey("AdvertisementItemId")]
		public virtual AdvertisementItem AdvertisementItem { get; set; }
	}
}
