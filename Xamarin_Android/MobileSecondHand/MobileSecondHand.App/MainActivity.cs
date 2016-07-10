using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using Microsoft.AspNet.SignalR.Client;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Location;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MobileSecondHand.App
{
	[Activity(Label = "Lista og�osze�")]
	public class MainActivity : AppCompatActivity, IInfiniteScrollListener
	{
		RecyclerView advertisementsRecyclerView;
		AdvertisementItemListAdapter advertisementItemListAdapter;
		IAdvertisementItemService advertisementItemService;
		GpsLocationService gpsLocationService;
		SharedPreferencesHelper sharedPreferencesHelper;
		int advertisementsPage;
		private ProgressDialogHelper progress;
		private DrawerLayout drawerLayout;

		public MainActivity()
		{
			this.advertisementItemService = new AdvertisementItemService();
		}

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.gpsLocationService = new GpsLocationService(this, null);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(this);

			SetContentView(Resource.Layout.MainActivity);




			SetupToolbar();
			advertisementsPage = savedInstanceState == null ? 0 : savedInstanceState.GetInt(ExtrasKeys.ADVERTISEMENTS_LIST_PAGE);
			await SetupViews(savedInstanceState != null);
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (!MessengerService.ServiceIsRunning)
			{
				StartService(new Intent(this, typeof(MessengerService)));
			}

		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			SharedObject.Data = this.advertisementItemListAdapter.AdvertisementItems;
			outState.PutInt(ExtrasKeys.ADVERTISEMENTS_LIST_PAGE, advertisementsPage);
			base.OnSaveInstanceState(outState);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.mainActivityMenu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var handled = false;
			switch (item.ItemId)
			{
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

		public async void OnInfiniteScroll()
		{
			await DownloadAndShowAdvertisements(false);
		}


		private void SetupToolbar()
		{
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

			// Init toolbar
			var toolbar = FindViewById<Toolbar>(Resource.Id.app_bar);
			SetSupportActionBar(toolbar);
			//SupportActionBar.SetTitle(Resource.String.app);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);

			// Attach item selected handler to navigation view
			var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

			// Create ActionBarDrawerToggle button and add it to the toolbar
			var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
			drawerLayout.SetDrawerListener(drawerToggle);
			drawerToggle.SyncState();

			//load default home screen
			//var ft = FragmentManager.BeginTransaction();
			//ft.AddToBackStack(null);
			//ft.Add(Resource.Id.HomeFrameLayout, new HomeFragment());
			//ft.Commit();



			//var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			//SetSupportActionBar(toolbar);
			//SupportActionBar.Title = "Lista og�osze�";
			//SetActionBar(toolbar);
			//ActionBar.Title = "Lista og�osze�";
		}

		private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
		{

			switch (e.MenuItem.ItemId)
			{
				case (Resource.Id.nav_home):
					Toast.MakeText(this, "Home selected!", ToastLength.Short).Show();
					break;
				case (Resource.Id.navmessages):
					Toast.MakeText(this, "Message selected!", ToastLength.Short).Show();
					break;
				case (Resource.Id.navfriends):
					// React on 'Friends' selection
					break;
			}
			// Close drawer
			drawerLayout.CloseDrawers();
		}

		private async void RefreshAdvertisementList()
		{
			advertisementItemListAdapter.InfiniteScrollDisabled = false;
			await DownloadAndShowAdvertisements(true);
		}

		private async Task SetupViews(bool screenOrientationChaged)
		{
			progress = new ProgressDialogHelper(this);
			SetupFab();
			advertisementsRecyclerView = FindViewById<RecyclerView>(Resource.Id.advertisementsRecyclerView);
			var mLayoutManager = new LinearLayoutManager(this);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
			await DownloadAndShowAdvertisements(screenOrientationChaged ? false : true, screenOrientationChaged);
		}

		private async Task DownloadAndShowAdvertisements(bool resetList, bool screenOrientationChaged = false)
		{
			progress.ShowProgressDialog("Pobieranie og�osze�. Prosz� czeka�...");
			SetAdvertisementListPageNumber(resetList, screenOrientationChaged);
			List<AdvertisementItemShort> advertisements = await GetStoredOrDownloadAdvertisements(screenOrientationChaged);

			if (advertisements != null && advertisements.Count > 0)
			{
				if (advertisementItemListAdapter == null || resetList)
				{
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, advertisements, this);
					advertisementItemListAdapter.AdvertisementItemClick += AdvertisementItemListAdapter_AdvertisementItemClick;
					advertisementsRecyclerView.SetAdapter(advertisementItemListAdapter);
				}
				else
				{
					advertisementItemListAdapter.AddAdvertisements(advertisements);
				}
			}
			else
			{
				if (advertisementItemListAdapter == null)
				{
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, new List<AdvertisementItemShort>(), this);
				}
				advertisementItemListAdapter.InfiniteScrollDisabled = true;
			}
			progress.CloseProgressDialog();
		}

		private void SetAdvertisementListPageNumber(bool resetList, bool screenOrientationChaged)
		{
			if (resetList)
			{
				advertisementsPage = 0;
			}
			else if (!screenOrientationChaged)
			{
				advertisementsPage++;
			}
		}

		private async Task<List<AdvertisementItemShort>> GetStoredOrDownloadAdvertisements(bool screenOrientationChaged)
		{
			List<AdvertisementItemShort> advertisements;
			if (screenOrientationChaged)
			{
				advertisements = (List<AdvertisementItemShort>)SharedObject.Data;
			}
			else
			{
				advertisements = await GetAdvertisements();
			}

			return advertisements;
		}

		private void AdvertisementItemListAdapter_AdvertisementItemClick(object sender, ShowAdvertisementDetailsEventArgs eventArgs)
		{
			//przej�cie do widoku detalue dokumentu
			var intent = new Intent(this, typeof(AdvertisementItemDetailsActivity));
			intent.PutExtra(ExtrasKeys.ADVERTISEMENT_ITEM_ID, eventArgs.Id);
			intent.PutExtra(ExtrasKeys.ADVERTISEMENT_ITEM_DISTANCE, eventArgs.Distance);
			StartActivity(intent);
		}

		private async Task<List<AdvertisementItemShort>> GetAdvertisements()
		{
			var searchModel = new SearchModel();
			searchModel.CoordinatesModel = this.gpsLocationService.GetCoordinatesModel();
			searchModel.Page = advertisementsPage;
			var tokenModel = new TokenModel();
			tokenModel.Token = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);

			var list = await this.advertisementItemService.GetAdvertisements(searchModel, tokenModel);
			return list;
		}

		private void SetupFab()
		{
			var fab = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fab);
			fab.BringToFront();
			fab.Click += Fab_Click;
		}

		private void Fab_Click(object sender, EventArgs e)
		{
			AlertsService.ShowToast(this, "Bum!");
			var addAdvertisementIntent = new Intent(this, typeof(AddNewAdvertisementActivity));
			StartActivity(addAdvertisementIntent);
		}

	}
}