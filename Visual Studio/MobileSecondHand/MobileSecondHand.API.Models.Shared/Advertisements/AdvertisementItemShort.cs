﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
    public class AdvertisementItemShort
    {
		public int Id { get; set; }
		public string AdvertisementTitle { get; set; }
		public int AdvertisementPrice { get; set; }
		public double Distance { get; set; }
		public byte[] MainPhoto { get; set; }
		public bool IsSellerOnline { get; set; }
		public bool IsOnlyForSell { get; set; }
	}
}