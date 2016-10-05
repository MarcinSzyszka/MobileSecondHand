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
using MobileSecondHand.API.Models.Shared.Advertisements;

namespace MobileSecondHand.App
{
	[Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
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
		private RelativeLayout sortingOptionsLayout;
		private RelativeLayout mainListLayout;
		private CategoriesSelectingHelper categoriesHelper;
		AdvertisementsSearchModel advertisementsSearchModel;
		private TextView textViewSelectCategories;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetAdvertisementsListKind();
			this.gpsLocationService = new GpsLocationService(this, null);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.categoriesHelper = new CategoriesSelectingHelper(this);

			SetContentView(Resource.Layout.MainActivity);
			base.SetupToolbar(false);
			advertisementsPage = 0;
			await SetupViews();
			if (savedInstanceState != null)
			{
				//tu dodrobic odtwarzanie przy uzyciu JsonCOnvert
				this.advertisementsSearchModel = new AdvertisementsSearchModel();
			}
			else
			{
				this.advertisementsSearchModel = new AdvertisementsSearchModel();
			}
			SetupSortingViews();
			await DownloadAndShowAdvertisements(true);
		}

		private void SetupSortingViews()
		{
			SetupSelectedCategoryView();
		}

		private void SetupSelectedCategoryView()
		{
			var text = String.Empty;
			if (advertisementsSearchModel.CategoriesModel.Count == 0)
			{
				text = "Wszystkie kategorie";
			}
			else
			{
				var sb = new StringBuilder();
				foreach (var cat in advertisementsSearchModel.CategoriesModel)
				{
					sb.AppendLine(cat.Value);
				}

				text = sb.ToString();
			}

			this.textViewSelectCategories.Text = text;
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
			AlertsService.ShowSingleSelectListString(this, kindNames.ToArray(), methodAfterSelect(), this.advertisementsKind.GetDisplayName());

		}

		private Action<string> methodAfterSelect()
		{
			return async s =>
			{
				advertisementsKind = s.GetEnumValueByDisplayName<AdvertisementsKind>();
				this.advertisementsListKindTextView.Text = advertisementsKind.GetDisplayName();
				this.advertisementItemListAdapter.InfiniteScrollDisabled = false;
				await DownloadAndShowAdvertisements(true);
			};
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
			sortingOptionsLayout = FindViewById<RelativeLayout>(Resource.Id.layoutSortingOptions);
			mainListLayout = FindViewById<RelativeLayout>(Resource.Id.mainListLayout);
			var btnSelectCategories = FindViewById<ImageButton>(Resource.Id.btnSelectCategoryForMainList);
			this.textViewSelectCategories = FindViewById<TextView>(Resource.Id.textViewSelectedCategoryForMainList);

			btnSelectCategories.Click += async (s, e) =>
			{
				var userSelectesKeywordsNames = this.advertisementsSearchModel.CategoriesModel.Select(c => c.Value).ToList();
				await this.categoriesHelper.ShowCategoriesListAndMakeAction(userSelectesKeywordsNames, MethodToExecuteAfterCategoriesSelect);
			};
			var btnSorting = FindViewById<ImageButton>(Resource.Id.btnSorting);


			var mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
		}
		private Action<System.Collections.Generic.List<string>> MethodToExecuteAfterCategoriesSelect(System.Collections.Generic.IDictionary<int, string> allKeywords)
		{
			return selectedItemsNames =>
			{
				this.advertisementsSearchModel.CategoriesModel.Clear();
				if (selectedItemsNames.Count != allKeywords.Count)
				{
					foreach (var itemName in selectedItemsNames)
					{
						this.advertisementsSearchModel.CategoriesModel.Add(allKeywords.First(k => k.Value == itemName));
					}
				}

				SetupSelectedCategoryView();
			};
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
					advertisementItemListAdapter.DeleteAdvertisementItemClick += AdvertisementItemListAdapter_DeleteAdvertisementItemClick;
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

		private async void AdvertisementItemListAdapter_DeleteAdvertisementItemClick(object sender, int advertisementId)
		{
			if (advertisementId == 0)
			{
				AlertsService.ShowToast(this, "Wyst¹pi³ b³¹d");
				return;
			}

			var message = advertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser ? "Czy na pewno chcesz zakoñczyæ to og³oszenie? Przestanie byæ ono widoczne na liœcie og³oszeñ."
																								: "Czy na pewno chcesz wyrzuciæ ze schowka to og³oszenie?";

			AlertsService.ShowConfirmDialog(this, message, async () =>
			{
				var success = await this.advertisementItemService.DeleteAdvertisement(advertisementId, advertisementsKind);

				if (success)
				{
					AlertsService.ShowToast(this, "Pomyœlnie zakoñczono tê operacjê.");
					RefreshAdvertisementList();
				}
				else
				{
					AlertsService.ShowToast(this, "Nie uda³o siê wykonaæ tej operacji.");
				}
			});

		}



		private async Task<List<AdvertisementItemShort>> GetAdvertisements()
		{
			var userAdvertisements = false;
			var favouritesAdvertisements = false;
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
				case AdvertisementsKind.FavouritesAdvertisements:
					favouritesAdvertisements = true;
					break;
			}

			var list = default(List<AdvertisementItemShort>);

			if (!userAdvertisements && !favouritesAdvertisements)
			{
				list = await this.advertisementItemService.GetAdvertisements(searchModel);
			}
			else if (userAdvertisements)
			{
				list = await this.advertisementItemService.GetUserAdvertisements(this.advertisementsPage);
			}
			else
			{
				list = await this.advertisementItemService.GetUserFavouritesAdvertisements(this.advertisementsPage);
			}

			return list;
		}

		private void SetupFab()
		{
			var fab = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fab);
			fab.BringToFront();
			fab.Click += Fab_Click;

			var fabFilter = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fabFilter);
			fabFilter.BringToFront();
			fabFilter.Click += FabFilter_Click;

			var fabMainListOptions = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fabMainListOptions);
			fabMainListOptions.BringToFront();
			fabMainListOptions.Click += FabMainListOptions_Click; ;
		}

		private void FabMainListOptions_Click(object sender, EventArgs e)
		{
			TogleLayouts();
		}

		private void FabFilter_Click(object sender, EventArgs e)
		{
			TogleLayouts();
		}

		private void TogleLayouts()
		{
			if (mainListLayout.Visibility == ViewStates.Visible)
			{
				mainListLayout.Visibility = ViewStates.Gone;
				sortingOptionsLayout.Visibility = ViewStates.Visible;
			}
			else
			{
				sortingOptionsLayout.Visibility = ViewStates.Gone;
				mainListLayout.Visibility = ViewStates.Visible;
			}
		}

		private void Fab_Click(object sender, EventArgs e)
		{
			var addAdvertisementIntent = new Intent(this, typeof(AddNewAdvertisementActivity));
			StartActivity(addAdvertisementIntent);
		}

	}
}