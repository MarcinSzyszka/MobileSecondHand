using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Location;

namespace MobileSecondHand.App {
	[Activity(Label = "Lista og³oszeñ")]
	public class MainActivity : Activity, IAdvertisementsInfiniteScrollListener {
		RecyclerView advertisementsRecyclerView;
		AdvertisementItemListAdapter advertisementItemRecyclerView;
		IAdvertisementItemService advertisementItemService;
		GpsLocationService gpsLocationService;
		SharedPreferencesHelper sharedPreferencesHelper;
		int advertisementsPage = 0;
		private ProgressDialogHelper progress;

		public MainActivity() {
			this.advertisementItemService = new AdvertisementItemService();
		}

		protected override async void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			this.gpsLocationService = new GpsLocationService(this, null);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(this);

			SetContentView(Resource.Layout.MainActivity);
			SetupToolbar();
			await SetupViews();
		}

		private void SetupToolbar() {
			var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
			SetActionBar(toolbar);
			ActionBar.Title = "Lista og³oszeñ";
		}

		public override bool OnCreateOptionsMenu(IMenu menu) {
			MenuInflater.Inflate(Resource.Menu.mainActivityMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item) {
			var handled = false;
			switch (item.ItemId) {
				case Resource.Id.refreshAdvertisementsOption:
					this.RefreshAdvertisementList();
					handled = true;
					break;
				case Resource.Id.appSettingOptions:
					handled = true;
					break;

			}

			return handled;
		}

		private async void RefreshAdvertisementList() {
			advertisementItemRecyclerView.InfiniteScrollDisabled = false;
			await DownloadAndShowAdvertisements(true);
		}

		private async Task SetupViews() {
			progress = new ProgressDialogHelper(this);
			SetupFab();
			advertisementsRecyclerView = FindViewById<RecyclerView>(Resource.Id.advertisementsRecyclerView);
			var mLayoutManager = new LinearLayoutManager(this);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
			await DownloadAndShowAdvertisements(true);
		}

		private async Task DownloadAndShowAdvertisements(bool resetList) {
			progress.ShowProgressDialog("Pobieranie og³oszeñ. Proszê czekaæ...");
			advertisementsPage = resetList ? 0 : advertisementsPage + 1;
			List<AdvertisementItemShort> advertisements = await GetAdvertisements();
			if (advertisements != null && advertisements.Count > 0) {
				if (advertisementItemRecyclerView == null || resetList) {
					advertisementItemRecyclerView = new AdvertisementItemListAdapter(this, advertisements, this);
					advertisementItemRecyclerView.AdvertisementItemClick += AdvertisementItemListAdapter_AdvertisementItemClick;
					advertisementsRecyclerView.SetAdapter(advertisementItemRecyclerView);
				}
				else {
					advertisementItemRecyclerView.AddAdvertisements(advertisements);
				}
			}
			else {
				advertisementItemRecyclerView.InfiniteScrollDisabled = true;
			}
			progress.CloseProgressDialog();
		}

		private void AdvertisementItemListAdapter_AdvertisementItemClick(object sender, ShowAdvertisementDetailsEventArgs eventArgs) {
			//przejœcie do widoku detalue dokumentu
			var intent = new Intent(this, typeof(AdvertisementItemDetailsActivity));
			intent.PutExtra(ExtrasKeys.ADVERTISEMENT_ITEM_ID, eventArgs.Id);
			intent.PutExtra(ExtrasKeys.ADVERTISEMENT_ITEM_DISTANCE, eventArgs.Distance);
			StartActivity(intent);
		}

		private async Task<List<AdvertisementItemShort>> GetAdvertisements() {
			var searchModel = new SearchModel();
			searchModel.CoordinatesModel = this.gpsLocationService.GetCoordinatesModel();
			searchModel.Page = advertisementsPage;
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
			var addAdvertisementIntent = new Intent(this, typeof(AddNewAdvertisementActivity));
			StartActivity(addAdvertisementIntent);
		}

		public async void OnInfiniteScroll() {
			await DownloadAndShowAdvertisements(false);
		}
	}
}