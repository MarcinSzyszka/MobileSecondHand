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
using MobileSecondHand.App.Activities;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.App.Notifications;
using MobileSecondHand.App.SideMenu;
using MobileSecondHand.Common.Enumerations;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Location;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using MobileSecondHand.Common.Extensions;
using Newtonsoft.Json;

namespace MobileSecondHand.App
{
	[Activity(Label = "Lista og³oszeñ", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : BaseActivity, IInfiniteScrollListener
	{
		RecyclerView advertisementsRecyclerView;
		AdvertisementItemListAdapter advertisementItemListAdapter;
		IAdvertisementItemService advertisementItemService;
		GpsLocationService gpsLocationService;
		int advertisementsPage;
		private ProgressDialogHelper progress;
		AdvertisementsKind advertisementsKind;
		private TextView advertisementsListKindTextView;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetAdvertisementsListKind();
			this.gpsLocationService = new GpsLocationService(this, null);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);

			SetContentView(Resource.Layout.MainActivity);
			base.SetupToolbar();
			//advertisementsPage = savedInstanceState == null ? 0 : savedInstanceState.GetInt(ExtrasKeys.ADVERTISEMENTS_LIST_PAGE);
			advertisementsPage = 0;
			await SetupViews();
		}

		private void SetAdvertisementsListKind()
		{
			var kindExtra = Intent.GetStringExtra(ExtrasKeys.NEW_ADVERTISEMENT_KIND);
			if (kindExtra != null)
			{
				advertisementsKind = JsonConvert.DeserializeObject<AdvertisementsKind>(kindExtra);
			}
			else
			{
				advertisementsKind = AdvertisementsKind.AdvertisementsAroundUserCurrentLocation;
			}

		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			//SharedObject.Data = this.advertisementItemListAdapter.AdvertisementItems;
			//outState.PutInt(ExtrasKeys.ADVERTISEMENTS_LIST_PAGE, advertisementsPage);
			base.OnSaveInstanceState(outState);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.mainActivityMenu, menu);
			if (menu != null)
			{
				menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(true);
				menu.FindItem(Resource.Id.chat).SetVisible(true);
				menu.FindItem(Resource.Id.choosingAdvertisementsList).SetVisible(true);
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
					var intent = new Intent(this, typeof(ConversationsListActivity));
					StartActivity(intent);
					handled = true;
					break;
				case Resource.Id.choosingAdvertisementsList:
					ShowChoosingAdvertisementsKindDialog();
					handled = true;
					break;
			}

			return handled;
		}

		private void ShowChoosingAdvertisementsKindDialog()
		{
			var kindNames = Enum.GetValues(typeof(AdvertisementsKind)).GetAllItemsDisplayNames();
			AlertsService.ShowSingleSelectListString(this, kindNames.ToArray(), async s =>
			{
				advertisementsKind = s.GetEnumValueByDisplayName<AdvertisementsKind>();
				this.advertisementsListKindTextView.Text = advertisementsKind.GetDisplayName();
				this.advertisementItemListAdapter.InfiniteScrollDisabled = false;
				await DownloadAndShowAdvertisements(true);
			});

		}

		public async void OnInfiniteScroll()
		{
			try
			{
				await DownloadAndShowAdvertisements(false);
			}
			catch (Exception)
			{

				//throw;
			}

		}


		private async void RefreshAdvertisementList()
		{
			advertisementItemListAdapter.InfiniteScrollDisabled = false;
			try
			{
				await DownloadAndShowAdvertisements(true);
			}
			catch (Exception)
			{

				//throw;
			}

		}

		private async Task SetupViews()
		{
			progress = new ProgressDialogHelper(this);
			SetupFab();
			advertisementsListKindTextView = FindViewById<TextView>(Resource.Id.advertisementsKindList);
			advertisementsListKindTextView.Text = advertisementsKind.GetDisplayName();
			advertisementsRecyclerView = FindViewById<RecyclerView>(Resource.Id.advertisementsRecyclerView);
			var mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
			try
			{
				await DownloadAndShowAdvertisements(true);
			}
			catch (Exception)
			{
				//throw;
			}

		}

		private async Task DownloadAndShowAdvertisements(bool resetList)
		{
			progress.ShowProgressDialog("Pobieranie og³oszeñ. Proszê czekaæ...");
			SetAdvertisementListPageNumber(resetList);
			List<AdvertisementItemShort> advertisements = await GetAdvertisements();

			if (advertisements.Count > 0 || resetList)
			{
				if (advertisementItemListAdapter == null || resetList)
				{
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, advertisements, advertisementsKind, this);
					advertisementItemListAdapter.AdvertisementItemClick += AdvertisementItemListAdapter_AdvertisementItemClick;
					var mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
					advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
					advertisementsRecyclerView.SetAdapter(advertisementItemListAdapter);
					advertisementsRecyclerView.RequestLayout();
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
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, new List<AdvertisementItemShort>(), advertisementsKind, this);
				}
				advertisementItemListAdapter.InfiniteScrollDisabled = true;
			}
			progress.CloseProgressDialog();
		}

		private void SetAdvertisementListPageNumber(bool resetList)
		{
			if (resetList)
			{
				advertisementsPage = 0;
			}
			else
			{
				advertisementsPage++;
			}
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
			var userAdvertisements = false;
			var searchModel = new SearchAdvertisementsModel();
			searchModel.Page = advertisementsPage;

			switch (advertisementsKind)
			{
				case AdvertisementsKind.AdvertisementsAroundUserCurrentLocation:
					try
					{
						searchModel.CoordinatesModel = this.gpsLocationService.GetCoordinatesModel();
					}
					catch (Exception exc)
					{
						return new List<AdvertisementItemShort>();
					}

					break;
				case AdvertisementsKind.AdvertisementsArounUserHomeLocation:
					var settingsMOdel = (AppSettingsModel)sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
					if (settingsMOdel != null)
					{
						searchModel.CoordinatesModel = settingsMOdel.LocationSettings;
						if (searchModel.CoordinatesModel.Latitude == 0.0D)
						{
							AlertsService.ShowToast(this, "Nie masz ustawionej lokalizacji domowej. Mo¿esz to zrobiæ w lewym panelu");
							return new List<AdvertisementItemShort>();
						}
					}
					else
					{
						AlertsService.ShowToast(this, "Nie masz ustawionej lokalizacji domowej. Mo¿esz to zrobiæ w lewym panelu");
						return new List<AdvertisementItemShort>();
					}

					break;
				case AdvertisementsKind.AdvertisementsCreatedByUser:
					userAdvertisements = true;
					break;
			}

			var list = default(List<AdvertisementItemShort>);

			if (!userAdvertisements)
			{
				list = await this.advertisementItemService.GetAdvertisements(searchModel);
			}
			else
			{
				list = await this.advertisementItemService.GetUserAdvertisements(this.advertisementsPage);
			}

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
			var addAdvertisementIntent = new Intent(this, typeof(AddNewAdvertisementActivity));
			StartActivity(addAdvertisementIntent);
		}

	}
}