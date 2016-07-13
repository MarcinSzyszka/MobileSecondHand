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

			// Create ActionBarDrawerToggle button and add it to the toolbar
			var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
			drawerLayout.SetDrawerListener(drawerToggle);
			drawerToggle.SyncState();
			drawerLayout.DrawerOpened += DrawerLayout_DrawerOpened;

			//load default home screen
			//var ft = FragmentManager.BeginTransaction();
			//ft.AddToBackStack(null);
			//ft.Add(Resource.Id.HomeFrameLayout, new HomeFragment());
			//ft.Commit();



			//var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			//SetSupportActionBar(toolbar);
			//SupportActionBar.Title = "Lista og³oszeñ";
			//SetActionBar(toolbar);
			//ActionBar.Title = "Lista og³oszeñ";
		}

		private void DrawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
		{
			AppSettingsModel settingsModel = GetAppSettings();
			if (navigationViewMenu == null)
			{
				navigationViewMenu = new NavigationViewMenu(this);
			}

			navigationViewMenu.SetupMenu(settingsModel);
		}

		private void SetHomeLocationSettings(AppSettingsModel settingsModel)
		{
			//this.textViewHomeLocalization.Text = settingsModel.LocationSettings.Latitude > 0 ? String.Format("Lat {0}, Lon {1}", settingsModel.LocationSettings.Latitude, settingsModel.LocationSettings.Longitude) : "nieustawiona";
		}

		private void SetKeywordsSettings(AppSettingsModel settingsModel)
		{
			//if (settingsModel.Keywords.Count > 0)
			//{
			//	var sb = new StringBuilder("");
			//	foreach (var keyword in settingsModel.Keywords)
			//	{
			//		sb.Append(keyword.Value);
			//		sb.Append("\r\n");
			//	}
			//	//this.textViewKeywords.Text = sb.ToString();
			//}
			//else
			//{
			//	this.textViewKeywords.Text = "Wszystkie";
			//}

			//this.textViewNotificationsRadius.Text = settingsModel.LocationSettings.MaxDistance > 0 ? String.Format("{0} km", settingsModel.LocationSettings.MaxDistance.ToString()) : "bez ograniczeñ";
		}

		private AppSettingsModel GetAppSettings()
		{
			var settingsModel = (AppSettingsModel)this.sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
			if (settingsModel == null)
			{
				settingsModel = new AppSettingsModel();
				this.sharedPreferencesHelper.SetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS, settingsModel);
			}

			return settingsModel;
		}

		private void SetNotificationsSettings()
		{
			//if (NewsService.ServiceIsRunnig)
			//{
			//	this.textViewNotificationsState.Text = "w³¹czone";
			//	this.imgBtnNotificationsStartStop.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.kasowanie_setting));
			//}
			//else
			//{
			//	this.textViewNotificationsState.Text = "wy³¹czone";
			//	this.imgBtnNotificationsStartStop.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.enable_setting));
			//}
		}

		private void SetChatSettings()
		{
			//if (MessengerService.ServiceIsRunning)
			//{
			//	this.textViewChatState.Text = "w³¹czony";
			//	this.imgBtnChatStartStop.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.kasowanie_setting));
			//}
			//else
			//{
			//	this.textViewChatState.Text = "wy³¹czony";
			//	this.imgBtnChatStartStop.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.enable_setting));
			//}

			//this.imgBtnChatStartStop.Click += (sender, args) =>
			//{
			//	AlertsService.ShowConfirmDialog(this, "Czy na pewno chcesz wy³¹czyæ czat?", () =>
			//	{
			//		StopService(new Intent(this, typeof(MessengerService)));
			//		SetChatSettings();
			//	});
			//};
		}

		private void SetupNavigationView()
		{
			var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

			//if (textViewUserName != null)
			//{
			//	return;
			//}
			//this.textViewUserName = FindViewById<TextView>(Resource.Id.textViewUserName);
			//this.textViewNotificationsState = FindViewById<TextView>(Resource.Id.textViewNotificationsState);
			//this.textViewChatState = FindViewById<TextView>(Resource.Id.textViewChatState);
			//this.textViewNotificationsRadius = FindViewById<TextView>(Resource.Id.textViewNotificationsRadius);
			//this.textViewKeywords = FindViewById<TextView>(Resource.Id.textViewKeywords);
			//this.textViewHomeLocalization = FindViewById<TextView>(Resource.Id.textViewHomeLocalization);
			//this.imgBtnConversations = FindViewById<ImageButton>(Resource.Id.imgBtnConversations);
			//this.imgBtnChatStartStop = FindViewById<ImageButton>(Resource.Id.imgBtnChatStartStop);
			//this.imgBtnNotificationsStartStop = FindViewById<ImageButton>(Resource.Id.imgBtnNotificationsStartStop);
			//this.imgBtnRadius = FindViewById<ImageButton>(Resource.Id.imgBtnRadius);
			//this.imgBtnKeywords = FindViewById<ImageButton>(Resource.Id.imgBtnKeywords);
			//this.imgBtnHomeLocalization = FindViewById<ImageButton>(Resource.Id.imgBtnHomeLocalization);
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