using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.Services.Advertisements
{
	public class AdvertisementSearchModelCopier
	{
		private AdvertisementsSearchModel originalSearchModel;

		TransactionKind TransactionKind { get; set; } = TransactionKind.All;
		bool ExpiredAdvertisements { get; set; }
		SortingBy SortingBy { get; set; } = SortingBy.sortByNearest;
		List<ClothSize> Sizes { get; set; } = new List<ClothSize>();
		IDictionary<int, string> CategoriesModel { get; set; } = new Dictionary<int, string>();
		AdvertisementsKind AdvertisementsKind { get; set; }
		CoordinatesForAdvertisementsModel CoordinatesModel { get; set; } = new CoordinatesForAdvertisementsModel();
		int Page { get; set; }
		UserInfoModel UserInfo { get; set; }

		public AdvertisementSearchModelCopier(AdvertisementsSearchModel searchModel)
		{
			this.originalSearchModel = searchModel;
			TransactionKind = searchModel.TransactionKind;
			ExpiredAdvertisements = searchModel.ExpiredAdvertisements;
			SortingBy = searchModel.SortingBy;
			Sizes.AddRange(searchModel.Sizes);
			foreach (var cat in searchModel.CategoriesModel)
			{
				CategoriesModel.Add(cat);
			}
			AdvertisementsKind = searchModel.AdvertisementsKind;
			CoordinatesModel.Latitude = searchModel.CoordinatesModel.Latitude;
			CoordinatesModel.LocationAddress = searchModel.CoordinatesModel.LocationAddress;
			CoordinatesModel.Longitude = searchModel.CoordinatesModel.Longitude;
			CoordinatesModel.MaxDistance = searchModel.CoordinatesModel.MaxDistance;
			Page = searchModel.Page;
			if (searchModel.UserInfo != null)
			{
				UserInfo.Id = searchModel.UserInfo.Id;
				UserInfo.UserName = searchModel.UserInfo.UserName;
			}
		}

		public AdvertisementsSearchModel RestorePreviousValues()
		{
			var searchModel = new AdvertisementsSearchModel();
			searchModel.TransactionKind = TransactionKind;
			searchModel.ExpiredAdvertisements = ExpiredAdvertisements;
			searchModel.SortingBy = SortingBy;
			searchModel.Sizes.AddRange(Sizes);
			foreach (var cat in CategoriesModel)
			{
				CategoriesModel.Add(cat);
			}
			searchModel.AdvertisementsKind = AdvertisementsKind;
			searchModel.CoordinatesModel.Latitude = CoordinatesModel.Latitude;
			searchModel.CoordinatesModel.LocationAddress = CoordinatesModel.LocationAddress;
			searchModel.CoordinatesModel.Longitude = CoordinatesModel.Longitude;
			searchModel.CoordinatesModel.MaxDistance = CoordinatesModel.MaxDistance;
			searchModel.Page = Page;
			if (UserInfo != null)
			{
				searchModel.UserInfo = new UserInfoModel();
				searchModel.UserInfo.Id = UserInfo.Id;
				searchModel.UserInfo.UserName = UserInfo.UserName;
			}

			return searchModel;
		}

		public bool IsSearchModelChanged()
		{
			if (originalSearchModel.TransactionKind != TransactionKind)
			{
				return true;
			}
			if (originalSearchModel.ExpiredAdvertisements != ExpiredAdvertisements)
			{
				return true;
			}
			if (originalSearchModel.SortingBy != SortingBy)
			{
				return true;
			}
			if (originalSearchModel.Sizes.Count != Sizes.Count)
			{
				return true;
			}
			if (originalSearchModel.CategoriesModel.Count != CategoriesModel.Count)
			{
				return true;
			}
			if (originalSearchModel.AdvertisementsKind != AdvertisementsKind)
			{
				return true;
			}
			if (originalSearchModel.TransactionKind != TransactionKind)
			{
				return true;
			}
			if (originalSearchModel.Page != Page)
			{
				return true;
			}
			if (originalSearchModel.UserInfo == null && UserInfo != null)
			{
				return true;
			}

			if (originalSearchModel.UserInfo != null && UserInfo == null)
			{
				return true;
			}
			if (originalSearchModel.UserInfo != null && UserInfo != null)
			{
				if (originalSearchModel.UserInfo.Id != UserInfo.Id)
				{
					return true;
				}
			}

			return false;
		}
	}



}
