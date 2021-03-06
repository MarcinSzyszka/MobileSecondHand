﻿using System.Collections.Generic;
using MobileSecondHand.API.Models.Shared.Consts;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.API.Models.Shared.Advertisements
{
	public class AdvertisementsSearchModel
	{
		public TransactionKind TransactionKind { get; set; } = TransactionKind.All;
		public bool ExpiredAdvertisements { get; set; }
		public SortingBy SortingBy { get; set; } = SortingBy.sortByNearest;
		public List<ClothSize> Sizes { get; set; } = new List<ClothSize>();
		public IDictionary<int, string> CategoriesModel { get; set; } = new Dictionary<int, string>();
		public AdvertisementsKind AdvertisementsKind { get; set; }
		public CoordinatesForAdvertisementsModel CoordinatesModel { get; set; } = new CoordinatesForAdvertisementsModel();
		public int Page { get; set; }
		public UserInfoModel UserInfo { get; set; }

		public AdvertisementsSearchModel()
		{
			CoordinatesModel.MaxDistance = ValueConsts.MAX_DISTANCE_VALUE;
		}
	}
}
