using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileSecondHand.App.Consts {
	public class SharedPreferencesKeys {
		public const string BEARER_TOKEN = "bearerToken";
		public const string HOME_LATITUDE = "homeLatitude";
		public const string HOME_LONGITUDE = "homeLongitude";
		public const string DISTANCE_FORADVERTISEMENT = "distanceForAdvertisements";
		public const string APP_SETTINGS = "appSettings";
	}
}