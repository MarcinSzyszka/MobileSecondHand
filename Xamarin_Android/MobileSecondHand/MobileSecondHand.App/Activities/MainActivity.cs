using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Activities;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Advertisements;
using Newtonsoft.Json;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Extensions;

namespace MobileSecondHand.App
{
	[Activity(LaunchMode = Android.Content.PM.LaunchMode.SingleInstance, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : BaseActivity, IInfiniteScrollListener
	{
		RecyclerView advertisementsRecyclerView;
		AdvertisementItemListAdapter advertisementItemListAdapter;
		IAdvertisementItemService advertisementItemService;
		GpsLocationService gpsLocationService;
		int advertisementsPage;
		private ProgressDialogHelper progress;
		private TextView advertisementsListKindTextView;
		private RelativeLayout sortingOptionsLayout;
		private RelativeLayout mainListLayout;
		private CategoriesSelectingHelper categoriesHelper;
		AdvertisementsSearchModel advertisementsSearchModel;
		private TextView textViewSelectCategories;
		private com.refractored.fab.FloatingActionButton fabOpenFilterOptions;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.gpsLocationService = new GpsLocationService(this, null);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.categoriesHelper = new CategoriesSelectingHelper(this);
			SetContentView(Resource.Layout.MainActivity);
			base.SetupToolbar(false);
			advertisementsPage = 0;

			if (savedInstanceState != null)
			{
				//tu dodrobic odtwarzanie przy uzyciu JsonCOnvert
				this.advertisementsSearchModel = new AdvertisementsSearchModel();
			}
			else
			{
				this.advertisementsSearchModel = new AdvertisementsSearchModel();
			}
			SetAdvertisementsListKind();
			await SetupViews();
			SetupSortingViews();
			await DownloadAndShowAdvertisements(true);
		}

		public override void OnBackPressed()
		{
			if (sortingOptionsLayout.Visibility == ViewStates.Invisible)
			{
				TogleLayouts();
			}
			else
			{
				base.OnBackPressed();
			}
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
				this.advertisementsSearchModel.AdvertisementsKind = JsonConvert.DeserializeObject<AdvertisementsKind>(kindExtra);
			}
			else
			{
				this.advertisementsSearchModel.AdvertisementsKind = AdvertisementsKind.AdvertisementsAroundUserCurrentLocation;
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
			AlertsService.ShowSingleSelectListString(this, kindNames.ToArray(), methodAfterSelect(), this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName());

		}

		private Action<string> methodAfterSelect()
		{
			return async s =>
			{
				this.advertisementsSearchModel.AdvertisementsKind = s.GetEnumValueByDisplayName<AdvertisementsKind>();
				if (this.advertisementsSearchModel.AdvertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser || this.advertisementsSearchModel.AdvertisementsKind == AdvertisementsKind.FavouritesAdvertisements)
				{
					ClearAdvertisementSearchAndHideFilterFabModel();
					this.fabOpenFilterOptions.Visibility = ViewStates.Invisible;
				}
				else
				{
					this.fabOpenFilterOptions.Visibility = ViewStates.Invisible;
				}
				this.advertisementsListKindTextView.Text = this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName();
				this.advertisementItemListAdapter.InfiniteScrollDisabled = false;
				await DownloadAndShowAdvertisements(true);
			};
		}

		private void ClearAdvertisementSearchAndHideFilterFabModel()
		{
			this.advertisementsSearchModel.CategoriesModel.Clear();
			ChangeFabOpenFilterOptionsDependsOnSelectedOptions();
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
			advertisementsListKindTextView.Text = this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName();
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
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, advertisements, this.advertisementsSearchModel.AdvertisementsKind, this);
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
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, new List<AdvertisementItemShort>(), this.advertisementsSearchModel.AdvertisementsKind, this);
				}
				advertisementItemListAdapter.InfiniteScrollDisabled = true;
			}
			progress.CloseProgressDialog();
		}

		private async Task<List<AdvertisementItemShort>> GetAdvertisements()
		{
			this.advertisementsSearchModel.Page = advertisementsPage;

			switch (this.advertisementsSearchModel.AdvertisementsKind)
			{
				case AdvertisementsKind.AdvertisementsAroundUserCurrentLocation:
					try
					{
						this.advertisementsSearchModel.CoordinatesModel = this.gpsLocationService.GetCoordinatesModel();
					}
					catch (Exception exc)
					{
						return new List<AdvertisementItemShort>();
					}

					break;
				case AdvertisementsKind.AdvertisementsArounUserHomeLocation:
					var settingsMOdel = (AppSettingsModel)sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
					if (settingsMOdel != null && settingsMOdel.LocationSettings.Latitude > 0.0D)
					{
						this.advertisementsSearchModel.CoordinatesModel = settingsMOdel.LocationSettings;
					}
					else
					{
						AlertsService.ShowToast(this, "Nie masz ustawionej lokalizacji domowej. Mo¿esz to zrobiæ w lewym panelu");
						return new List<AdvertisementItemShort>();
					}

					break;
			}



			var list = default(List<AdvertisementItemShort>);

			list = await this.advertisementItemService.GetAdvertisements(this.advertisementsSearchModel);

			return list;
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

			var message = this.advertisementsSearchModel.AdvertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser ? "Czy na pewno chcesz zakoñczyæ to og³oszenie? Przestanie byæ ono widoczne na liœcie og³oszeñ."
																								: "Czy na pewno chcesz wyrzuciæ ze schowka to og³oszenie?";

			AlertsService.ShowConfirmDialog(this, message, async () =>
			{
				var success = await this.advertisementItemService.DeleteAdvertisement(advertisementId, this.advertisementsSearchModel.AdvertisementsKind);

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



		private void SetupFab()
		{
			var fab = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fab);
			fab.BringToFront();
			fab.Click += Fab_Click;

			fabOpenFilterOptions = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fabFilter);
			fabOpenFilterOptions.BringToFront();
			fabOpenFilterOptions.Click += FabFilter_Click;

			var fabApplySortingOptions = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fabMainListOptions);
			fabApplySortingOptions.BringToFront();
			fabApplySortingOptions.Click += FabMainListOptions_Click; ;
		}

		private void FabMainListOptions_Click(object sender, EventArgs e)
		{
			TogleLayouts();
			ChangeFabOpenFilterOptionsDependsOnSelectedOptions();
		}

		private void ChangeFabOpenFilterOptionsDependsOnSelectedOptions()
		{
			var optionsSelected = false;
			if (this.advertisementsSearchModel.CategoriesModel.Count > 0)
			{
				optionsSelected = true;
			}

			if (optionsSelected)
			{
				fabOpenFilterOptions.SetImageResource(Resource.Drawable.filter_icon_full_digit);
			}
			else
			{
				fabOpenFilterOptions.SetImageResource(Resource.Drawable.filter_icon);
			}
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