﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Coordinates;

namespace MobileSecondHand.Api.Models.Advertisement
{
    public class SearchModel
    {
		public CoordinatesForAdvertisementsModel CoordinatesModel { get; set; }
		public int Page { get; set; }
	}
}
