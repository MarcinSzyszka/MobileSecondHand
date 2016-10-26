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
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Advertisements;
using Newtonsoft.Json;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Extensions;
using MobileSecondHand.App.Infrastructure.ActivityState;
using Android.Runtime;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.API.Models.Shared.Consts;
using Android.Support.V4.Widget;

namespace MobileSecondHand.App
{
	[Activity(Label = "", LaunchMode = Android.Content.PM.LaunchMode.SingleTask, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : BaseActivityWithNavigationDrawer, IInfiniteScrollListener
	{
		RecyclerView advertisementsRecyclerView;
		AdvertisementItemListAdapter advertisementItemListAdapter;
		IAdvertisementItemService advertisementItemService;
		GpsLocationService gpsLocationService;
		int advertisementsPage;
		private ProgressDialogHelper progress;
		private TextView advertisementsListKindTextView;
		private NestedScrollView sortingOptionsLayout;
		private RelativeLayout mainListLayout;
		private CategoriesSelectingHelper categoriesHelper;
		AdvertisementsSearchModel advertisementsSearchModel;
		private TextView textViewSelectCategories;
		private com.refractored.fab.FloatingActionButton fabOpenFilterOptions;
		private TextView textViewSelectedDistance;
		private TextView textViewSelectedUser;
		private TextView textViewSelectedSorting;
		private TextView textViewSelectedSize;
		private IMenu menu;
		private SizeSelectingHelper sizeSelectingHelper;
		private ImageView btnSize;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.gpsLocationService = new GpsLocationService(this, null);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.categoriesHelper = new CategoriesSelectingHelper(this, bearerToken);
			this.sizeSelectingHelper = new SizeSelectingHelper(this);
			SetContentView(Resource.Layout.MainActivity);
			SetupToolbar();
			SetupDrawer();
			advertisementsPage = 0;
			SetAdvertisementsListKind();
			SetSortingOptionsLayout();
			SetupViews();
			SetupSortingViews();
			await DownloadAndShowAdvertisements(true);
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok && requestCode == FindUserActivity.FIND_USER_REQUEST_CODE && data != null)
			{
				var userInfoModelString = data.GetStringExtra(ActivityStateConsts.USER_INFO_MODEL);
				var userInfoModel = JsonConvert.DeserializeObject<UserInfoModel>(userInfoModelString);
				advertisementsSearchModel.UserInfo = userInfoModel;
				SetupSortingViews();
			}
		}

		public override void OnBackPressed()
		{
			if (sortingOptionsLayout.Visibility != ViewStates.Visible)
			{
				base.OnBackPressed();
			}
		}

		private void SetupSortingViews()
		{
			SetupSelectedCategoryView();
			SetupSelectedUserView();
			SetupSelectedMaxDistanceView();
			SetupSelectedSortingByView();
			SetupSelectedSizesView();
		}

		private void SetupSelectedMaxDistanceView()
		{
			this.textViewSelectedDistance.Text = advertisementsSearchModel.CoordinatesModel.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE ? String.Format("{0} km", advertisementsSearchModel.CoordinatesModel.MaxDistance.ToString()) : "bez ograniczeñ";
		}

		private void SetupSelectedUserView()
		{
			if (advertisementsSearchModel.UserInfo == null)
			{
				this.textViewSelectedUser.Text = "Wszyscy";
			}
			else
			{
				this.textViewSelectedUser.Text = advertisementsSearchModel.UserInfo.UserName;
			}
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
			//sytuacja gdy activity jest wywo³ywane z serwisu sprawdzaj¹cego nowosci
			this.advertisementsSearchModel = new AdvertisementsSearchModel();
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
				this.menu = menu;
				menu.FindItem(Resource.Id.home).SetVisible(true);
				SetAppBarOptions(false);
			}

			return base.OnCreateOptionsMenu(menu);
		}

		private void SetAppBarOptions(bool isOnOptionsLayout)
		{
			if (isOnOptionsLayout)
			{
				this.menu.FindItem(Resource.Id.applyFilterOptions).SetVisible(true);
				this.menu.FindItem(Resource.Id.clearFilterOptions).SetVisible(true);
				this.menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(false);
				this.menu.FindItem(Resource.Id.chat).SetVisible(false);
				this.menu.FindItem(Resource.Id.choosingAdvertisementsList).SetVisible(false);
			}
			else
			{
				menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(true);
				menu.FindItem(Resource.Id.chat).SetVisible(true);
				menu.FindItem(Resource.Id.choosingAdvertisementsList).SetVisible(true);
				menu.FindItem(Resource.Id.applyFilterOptions).SetVisible(false);
				menu.FindItem(Resource.Id.clearFilterOptions).SetVisible(false);
			}


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
				case Resource.Id.clearFilterOptions:
					ClearFilterOptions();
					handled = true;
					break;
				case Resource.Id.applyFilterOptions:
					ApplyFilterOptions();
					handled = true;
					break;
			}

			return handled;
		}

		private void ShowChoosingAdvertisementsKindDialog()
		{
			Action<string> methodAfterSelect = async (s) =>
			{
				this.advertisementsSearchModel.AdvertisementsKind = s.GetEnumValueByDisplayName<AdvertisementsKind>();
				this.advertisementsListKindTextView.Text = this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName();
				this.advertisementItemListAdapter.InfiniteScrollDisabled = false;
				await DownloadAndShowAdvertisements(true);
			};
			var kindNames = Enum.GetValues(typeof(AdvertisementsKind)).GetAllItemsDisplayNames();
			AlertsService.ShowSingleSelectListString(this, kindNames.ToArray(), methodAfterSelect, this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName());

		}

		public async void OnInfiniteScroll()
		{
			await DownloadAndShowAdvertisements(false);
		}

		private async void RefreshAdvertisementList()
		{
			advertisementItemListAdapter.InfiniteScrollDisabled = false;
			await DownloadAndShowAdvertisements(true);
		}

		private void SetupViews()
		{
			progress = new ProgressDialogHelper(this);
			SetupFab();
			advertisementsListKindTextView = FindViewById<TextView>(Resource.Id.advertisementsKindList);
			advertisementsListKindTextView.Text = this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName();
			advertisementsRecyclerView = FindViewById<RecyclerView>(Resource.Id.advertisementsRecyclerView);
			mainListLayout = FindViewById<RelativeLayout>(Resource.Id.mainListLayout);
			var mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
		}

		private void SetSortingOptionsLayout()
		{
			sortingOptionsLayout = FindViewById<NestedScrollView>(Resource.Id.layoutSortingOptions);
			var btnSelectCategories = FindViewById<ImageView>(Resource.Id.btnSelectCategoryForMainList);
			this.textViewSelectCategories = FindViewById<TextView>(Resource.Id.textViewSelectedCategoryForMainList);

			btnSelectCategories.Click += async (s, e) =>
			{
				Func<System.Collections.Generic.IDictionary<int, string>, Action<System.Collections.Generic.List<string>>> methodToExecuteAfterCategoriesSelect = (allKeywords) =>
				{
					return (selectedItemsNames) =>
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
				};


				var userSelectesKeywordsNames = this.advertisementsSearchModel.CategoriesModel.Select(c => c.Value).ToList();
				await this.categoriesHelper.ShowCategoriesListAndMakeAction(userSelectesKeywordsNames, methodToExecuteAfterCategoriesSelect);
			};


			this.textViewSelectedSize = FindViewById<TextView>(Resource.Id.textViewSelectedSizes);
			this.btnSize = FindViewById<ImageView>(Resource.Id.btnSize);
			btnSize.Click += (s, e) =>
			{
				var selectedSizesNames = new List<String>();
				foreach (var size in this.advertisementsSearchModel.Sizes)
				{
					selectedSizesNames.Add(size.GetDisplayName());
				}
				Action<List<ClothSize>> actionAfterSelect = (selectedSizes) =>
				{
					this.advertisementsSearchModel.Sizes = selectedSizes;
					SetupSelectedSizesView();
				};
				this.sizeSelectingHelper.ShowSizesListAndMakeAction(selectedSizesNames, actionAfterSelect);
			};


			var btnDistance = FindViewById<ImageView>(Resource.Id.btnDistance);
			this.textViewSelectedDistance = FindViewById<TextView>(Resource.Id.textViewSelectedDistance);
			btnDistance.Click += (sender, args) =>
			{
				string[] itemList = Resources.GetStringArray(Resource.Array.notifications_radius);
				AlertsService.ShowSingleSelectListString(this, itemList, selectedText =>
				{
					var resultRadius = 0;
					var selectedRadius = selectedText.Split(new char[] { ' ' })[0];
					int.TryParse(selectedRadius, out resultRadius);
					if (resultRadius == 0)
					{
						resultRadius = ValueConsts.MAX_DISTANCE_VALUE;
					}
					advertisementsSearchModel.CoordinatesModel.MaxDistance = resultRadius;

					this.textViewSelectedDistance.Text = advertisementsSearchModel.CoordinatesModel.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE ? String.Format("{0} km", advertisementsSearchModel.CoordinatesModel.MaxDistance.ToString()) : "bez ograniczeñ";
				});
			};


			var btnSelectUser = FindViewById<ImageView>(Resource.Id.btnSelectUser);
			btnSelectUser.Click += (s, e) =>
			{
				var intent = new Intent(this, typeof(FindUserActivity));
				intent.PutExtra(ActivityStateConsts.CALLING_ACTIVITY_NAME, ActivityStateConsts.MAIN_ACTIVITY_NAME);
				StartActivityForResult(intent, FindUserActivity.FIND_USER_REQUEST_CODE);
			};
			this.textViewSelectedUser = FindViewById<TextView>(Resource.Id.textViewSelectedUser);

			var btnSorting = FindViewById<ImageView>(Resource.Id.btnSorting);
			this.textViewSelectedSorting = FindViewById<TextView>(Resource.Id.textViewSelectedSorting);
			btnSorting.Click += (s, e) =>
			{
				Action<string> actionAfterSelect = (selctedSortingByName) =>
				{
					this.advertisementsSearchModel.SortingBy = selctedSortingByName.GetEnumValueByDisplayName<SortingBy>();
					SetupSelectedSortingByView();
				};
				var sortingByNames = Enum.GetValues(typeof(SortingBy)).GetAllItemsDisplayNames();
				AlertsService.ShowSingleSelectListString(this, sortingByNames.ToArray(), actionAfterSelect, this.advertisementsSearchModel.SortingBy.GetDisplayName());
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
						this.advertisementsSearchModel.CoordinatesModel = this.gpsLocationService.GetCoordinatesModel(advertisementsSearchModel.CoordinatesModel.MaxDistance);
					}
					catch (Exception exc)
					{
						return new List<AdvertisementItemShort>();
					}

					break;
				case AdvertisementsKind.AdvertisementsArounUserHomeLocation:
					var settingsMOdel = SharedPreferencesHelper.GetAppSettings(this);
					if (settingsMOdel != null && settingsMOdel.LocationSettings.Latitude > 0.0D)
					{
						this.advertisementsSearchModel.CoordinatesModel.Latitude = settingsMOdel.LocationSettings.Latitude;
						this.advertisementsSearchModel.CoordinatesModel.Latitude = settingsMOdel.LocationSettings.Longitude;
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

		private void AdvertisementItemListAdapter_DeleteAdvertisementItemClick(object sender, int advertisementId)
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
		}

		private void ClearFilterOptions()
		{
			SetDefaultSearchOptions();
			SetupSortingViews();
		}

		private void SetDefaultSearchOptions()
		{
			this.advertisementsSearchModel.CategoriesModel.Clear();
			this.advertisementsSearchModel.Sizes.Clear();
			this.advertisementsSearchModel.UserInfo = null;
			this.advertisementsSearchModel.CoordinatesModel.MaxDistance = ValueConsts.MAX_DISTANCE_VALUE;
			this.advertisementsSearchModel.SortingBy = SortingBy.sortByNearest;
		}

		private void SetupSelectedSortingByView()
		{
			this.textViewSelectedSorting.Text = this.advertisementsSearchModel.SortingBy.GetDisplayName();
		}

		private void SetupSelectedSizesView()
		{
			if (this.advertisementsSearchModel.Sizes.Count == 0)
			{
				this.textViewSelectedSize.Text = "Wszystkie rozmiary";
			}
			else
			{
				this.textViewSelectedSize.Text = "";
				foreach (var size in this.advertisementsSearchModel.Sizes)
				{
					this.textViewSelectedSize.Text += size.GetDisplayName() + "\r\n";
				}
			}
		}

		private async void ApplyFilterOptions()
		{
			TogleLayouts();
			ChangeFabOpenFilterOptionsDependsOnSelectedOptions();
			await DownloadAndShowAdvertisements(true);
		}

		private void ChangeFabOpenFilterOptionsDependsOnSelectedOptions()
		{
			var optionsSelected = false;
			if (this.advertisementsSearchModel.CategoriesModel.Count > 0)
			{
				optionsSelected = true;
			}
			if (this.advertisementsSearchModel.UserInfo != null)
			{
				optionsSelected = true;
			}
			if (this.advertisementsSearchModel.CoordinatesModel.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE)
			{
				optionsSelected = true;
			}
			if (this.advertisementsSearchModel.SortingBy != SortingBy.sortByNearest)
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
				SetAppBarOptions(true);
				mainListLayout.Visibility = ViewStates.Gone;
				sortingOptionsLayout.Visibility = ViewStates.Visible;
			}
			else
			{
				SetAppBarOptions(false);
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