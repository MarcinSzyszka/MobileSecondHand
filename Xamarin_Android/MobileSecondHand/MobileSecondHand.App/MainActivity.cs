using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Location;

namespace MobileSecondHand.App {
	[Activity(Label = "MainActivity")]
	public class MainActivity : Activity {
		ListView advertisementsListView;
		AdvertisementItemListAdapter advertisementItemListAdapter;
		IAdvertisementItemService advertisementItemService;
		GpsLocationService gpsLocationService;
		SharedPreferencesHelper sharedPreferencesHelper;

		public MainActivity() {
			this.advertisementItemService = new AdvertisementItemService();
		}

		protected override async void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			this.gpsLocationService = new GpsLocationService(this, null);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(this);

			SetContentView(Resource.Layout.MainActivity);
			await SetupViews();
			// Create your application here
		}

		public override bool OnOptionsItemSelected(IMenuItem item) {
			return base.OnOptionsItemSelected(item);
		}

		public override bool OnCreateOptionsMenu(IMenu menu) {
			MenuInflater.Inflate(Resource.Menu.mainActivityMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		private async Task SetupViews() {
			SetupFab();
			advertisementsListView = FindViewById<ListView>(Resource.Id.advertisementsListView);
			List<AdvertisementItemShort> avertisements = await GetAdvertisements();
			if (avertisements != null) {
				advertisementItemListAdapter = new AdvertisementItemListAdapter(this, avertisements);
				advertisementsListView.Adapter = advertisementItemListAdapter;
			}
		}

		private async Task<List<AdvertisementItemShort>> GetAdvertisements() {
			var searchModel = new SearchModel();
			searchModel.CoordinatesModel = this.gpsLocationService.GetCoordinatesModel();
			searchModel.Page = 0;
			var tokenModel = new TokenModel();
			tokenModel.Token = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);

			var list = await this.advertisementItemService.GetAdvertisements(searchModel, tokenModel);
			return list;
		}

		private void SetupFab() {
			var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += Fab_Click;
		}

		private void Fab_Click(object sender, EventArgs e) {
			AlertsService.ShowToast(this, "Bum!");
		}

	}
}