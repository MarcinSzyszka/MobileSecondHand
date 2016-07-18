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
using Android.Support.V4.Content;
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
using MobileSecondHand.App.Notifications;
using MobileSecondHand.App.SideMenu;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Location;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MobileSecondHand.App
{
	[Activity(Label = "Lista og³oszeñ")]
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
		NavigationViewMenu navigationViewMenu;

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
			if (menu != null)
			{
				menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(true);
				menu.FindItem(Resource.Id.chat).SetVisible(true);
			}

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
				case Resource.Id.chat:
					handled = true;
					break;

			}

			return handled;
		}

		public override void OnBackPressed()
		{
			if (drawerLayout.IsDrawerOpen(FindViewById(Resource.Id.nav_view)))
			{
				drawerLayout.CloseDrawers();
			}
			else
			{
				base.OnBackPressed();
			}

		}

		public async void OnInfiniteScroll()
		{
			await DownloadAndShowAdvertisements(false);
		}


		private void SetupToolbar()
		{
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

			var toolbar = FindViewById<Toolbar>(Resource.Id.app_bar);
			SetSupportActionBar(toolbar);
			//SupportActionBar.SetTitle(Resource.String.app);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);

			// Create ActionBarDrawerToggle button and add it to the toolbar
			var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
			drawerLayout.SetDrawerListener(drawerToggle);
			drawerToggle.SyncState();
			drawerLayout.DrawerOpened += DrawerLayout_DrawerOpened;
			if (drawerLayout.IsDrawerOpen(FindViewById(Resource.Id.nav_view)))
			{
				SetupSideMenu();
			}
		}

		private void DrawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
		{
			SetupSideMenu();
		}

		private void SetupSideMenu()
		{
			if (navigationViewMenu == null)
			{
				navigationViewMenu = new NavigationViewMenu(this, this.sharedPreferencesHelper);
			}

			navigationViewMenu.SetupMenu();
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
			//var mLayoutManager = new LinearLayoutManager(this);
			var mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
			await DownloadAndShowAdvertisements(screenOrientationChaged ? false : true, screenOrientationChaged);
		}

		private async Task DownloadAndShowAdvertisements(bool resetList, bool screenOrientationChaged = false)
		{
			progress.ShowProgressDialog("Pobieranie og³oszeñ. Proszê czekaæ...");
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
			//przejœcie do widoku detalue dokumentu
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