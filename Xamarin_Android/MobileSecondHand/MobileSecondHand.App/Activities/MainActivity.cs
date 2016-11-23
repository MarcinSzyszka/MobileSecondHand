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
using MobileSecondHand.Services.Advertisements;
using Newtonsoft.Json;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Extensions;
using MobileSecondHand.App.Infrastructure.ActivityState;
using Android.Runtime;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.API.Models.Shared.Consts;
using Android.Support.V4.Widget;
using MobileSecondHand.App.SideMenu;

namespace MobileSecondHand.App
{
	[Activity(Label = "Og³oszenia", LaunchMode = Android.Content.PM.LaunchMode.SingleTask, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : BaseActivityWithNavigationDrawer, IInfiniteScrollListener, View.IOnClickListener
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
		SwipeRefreshLayout mainListSwipeLayout;
		private CategoriesSelectingHelper categoriesHelper;
		AdvertisementsSearchModel advertisementsSearchModel;
		private TextView textViewSelectCategories;
		private com.refractored.fab.FloatingActionButton fabOpenFilterOptions;
		private TextView textViewSelectedDistance;
		private TextView textViewSelectedUser;
		private TextView textViewSelectedSorting;
		private TextView textViewSelectedSize;
		private TextView textViewSelectedAdvertsStatus;
		private TextView textViewSelectedTransactionKind;
		private IMenu menu;
		private SizeSelectingHelper sizeSelectingHelper;
		private ImageView btnSize;
		string activeStatus = "Trwaj¹ce";
		string expiredStatus = "Zakoñczone";
		Action<bool> RefreshAdvertisementList;
		private AdvertisementSearchModelCopier searchModelCopier;
		private TextView textViewNoAdverts;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.gpsLocationService = new GpsLocationService(this, null);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.categoriesHelper = new CategoriesSelectingHelper(this, bearerToken);
			this.sizeSelectingHelper = new SizeSelectingHelper(this);
			SetRefreshAdvertisementAction();
			SetContentView(Resource.Layout.MainActivity);
			SetupToolbar();
			SetupDrawer();
			drawerToggle.ToolbarNavigationClickListener = this;
			advertisementsPage = 0;
			SetSortingOptionsLayout();
			SetupFab();
			SetAdvertisementsListKind();
			SetupViews();
			SetupSortingViews();
			RefreshAdvertisementList(true);
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);
			SetAdvertisementsListKind(intent);
			RefreshAdvertisementList(true);
		}

		protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok && requestCode == FindUserActivity.FIND_USER_REQUEST_CODE && data != null)
			{
				var userInfoModelString = data.GetStringExtra(ActivityStateConsts.USER_INFO_MODEL);
				var userInfoModel = JsonConvert.DeserializeObject<UserInfoModel>(userInfoModelString);
				advertisementsSearchModel.UserInfo = userInfoModel;
				SetupSortingViews();
			}
			else if (resultCode == Result.Ok && requestCode == NavigationViewMenu.PHOTO_REQUEST_KEY)
			{
				await navigationViewMenu.OnAddPhotoTequestResult(data);
			}
		}

		public override void OnBackPressed()
		{
			if (IsDrawerOpen())
			{
				base.OnBackPressed();
			}
			else if (sortingOptionsLayout.Visibility != ViewStates.Visible)
			{
				if (this.advertisementsSearchModel.AdvertisementsKind != AdvertisementsKind.AdvertisementsAroundUserCurrentLocation)
				{
					ShowAdvertisementList(AdvertisementsKind.AdvertisementsAroundUserCurrentLocation);
				}
				else
				{
					base.OnBackPressed();
				}
			}
			else
			{
				Action actionOnConfirm = ApplyFilterOptions;
				Action actionOnDismiss = () =>
				{
					this.advertisementsSearchModel = searchModelCopier.RestorePreviousValues();
					SetupSortingViews();
					ChangeFabOpenFilterOptionsDependsOnSelectedOptions();
					TogleLayouts();
				};
				if (searchModelCopier.IsSearchModelChanged())
				{
					AlertsService.ShowConfirmDialog(this, "Czy uwzglêdniæ dokonane zmiany w filtrach?", actionOnConfirm, actionOnDismiss);
				}
				else
				{
					TogleLayouts();
				}
			}
		}

		private void SetRefreshAdvertisementAction()
		{
			RefreshAdvertisementList = async (withDisplayedProgress) =>
			{
				if (withDisplayedProgress)
				{
					progress.ShowProgressDialog("Pobieranie og³oszeñ. Proszê czekaæ...");
				}
				else
				{
					mainListSwipeLayout.Refreshing = true;
				}

				try
				{
					await DownloadAndShowAdvertisements(true);
				}
				catch (Exception)
				{
				}
				finally
				{
					if (withDisplayedProgress)
					{
						progress.CloseProgressDialog();
					}
					else
					{
						mainListSwipeLayout.Refreshing = false;
					}
				}
			};
		}

		private void SetupSortingViews()
		{
			SetupSelectedCategoryView();
			SetupSelectedUserView();
			SetupSelectedMaxDistanceView();
			SetupSelectedSortingByView();
			SetupSelectedSizesView();
			SetupSelectedAdvertStatus();
			SetupSelectedTransactionKind();
		}

		private void SetupSelectedMaxDistanceView()
		{
			this.textViewSelectedDistance.Text = advertisementsSearchModel.CoordinatesModel.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE ? String.Format("{0} km", advertisementsSearchModel.CoordinatesModel.MaxDistance.ToString()) : "Bez ograniczeñ";
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

		private void SetAdvertisementsListKind(Intent intent = null)
		{
			//sytuacja gdy activity jest wywo³ywane z serwisu sprawdzaj¹cego nowosci
			if (this.advertisementsSearchModel == null)
			{
				this.advertisementsSearchModel = new AdvertisementsSearchModel();
			}

			string kindExtra;
			if (intent != null)
			{
				kindExtra = intent.GetStringExtra(ExtrasKeys.NEW_ADVERTISEMENT_KIND);
			}
			else
			{
				kindExtra = Intent.GetStringExtra(ExtrasKeys.NEW_ADVERTISEMENT_KIND);
			}

			if (kindExtra != null)
			{
				this.advertisementsSearchModel.AdvertisementsKind = JsonConvert.DeserializeObject<AdvertisementsKind>(kindExtra);
				this.advertisementsSearchModel.SortingBy = SortingBy.sortByNewest;
				ChangeFabOpenFilterOptionsDependsOnSelectedOptions();
			}

			if (this.advertisementsSearchModel.AdvertisementsKind == default(AdvertisementsKind))
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
				menu.FindItem(Resource.Id.home).SetVisible(false);
				SetAppBarOptions(false);
			}

			return base.OnCreateOptionsMenu(menu);
		}

		private void SetAppBarOptions(bool isOnOptionsLayout)
		{
			if (isOnOptionsLayout)
			{
				SupportActionBar.Title = "Filtry";
				menu.FindItem(Resource.Id.showFavouritesList).SetVisible(false);
				this.menu.FindItem(Resource.Id.applyFilterOptions).SetVisible(true);
				this.menu.FindItem(Resource.Id.clearFilterOptions).SetVisible(true);
				this.menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(false);
				this.menu.FindItem(Resource.Id.chat).SetVisible(false);
				this.menu.FindItem(Resource.Id.choosingAdvertisementsList).SetVisible(false);
			}
			else
			{
				SupportActionBar.Title = "Og³oszenia";
				menu.FindItem(Resource.Id.showFavouritesList).SetVisible(true);
				menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(false);
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
				//case Resource.Id.refreshAdvertisementsOption:
				//	RefreshAdvertisementList(true);
				//	handled = true;
				//	break;
				case Resource.Id.showFavouritesList:
					ShowAdvertisementList(AdvertisementsKind.FavouritesAdvertisements);
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

		private void ShowAdvertisementList(AdvertisementsKind advertisementsKind)
		{
			this.advertisementsSearchModel.AdvertisementsKind = advertisementsKind;
			this.advertisementsListKindTextView.Text = this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName();
			this.advertisementItemListAdapter.InfiniteScrollDisabled = false;
			RefreshAdvertisementList(true);
		}

		private void ShowChoosingAdvertisementsKindDialog()
		{
			Action<string> methodAfterSelect = (s) =>
			{
				ShowAdvertisementList(s.GetEnumValueByDisplayName<AdvertisementsKind>());
			};
			var kindNames = Enum.GetValues(typeof(AdvertisementsKind)).GetAllItemsDisplayNames();
			AlertsService.ShowSingleSelectListString(this, kindNames.ToArray(), methodAfterSelect, this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName(), "Wybierz listê og³oszeñ");

		}

		public async void OnInfiniteScroll()
		{
			progress.ShowProgressDialog("Pobieranie og³oszeñ. Proszê czekaæ...");
			await DownloadAndShowAdvertisements(false);
			progress.CloseProgressDialog();
		}

		private void SetupViews()
		{
			progress = new ProgressDialogHelper(this);
			textViewNoAdverts = FindViewById<TextView>(Resource.Id.textViewNoAdverts);
			textViewNoAdverts.Click += (s, e) => RefreshAdvertisementList(true);
			advertisementsListKindTextView = FindViewById<TextView>(Resource.Id.advertisementsKindList);
			advertisementsListKindTextView.Click += (s, e) => ShowChoosingAdvertisementsKindDialog();
			advertisementsListKindTextView.Text = this.advertisementsSearchModel.AdvertisementsKind.GetDisplayName();
			advertisementsRecyclerView = FindViewById<RecyclerView>(Resource.Id.advertisementsRecyclerView);
			mainListLayout = FindViewById<RelativeLayout>(Resource.Id.mainListLayout);
			mainListSwipeLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.mainListSwipeLayout);
			mainListSwipeLayout.Refresh += (s, e) =>
			{
				RefreshAdvertisementList(false);
			};
			var mLayoutManager = new GridLayoutManager(this, 2);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);
		}


		private void SetSortingOptionsLayout()
		{
			sortingOptionsLayout = FindViewById<NestedScrollView>(Resource.Id.layoutSortingOptions);

			textViewSelectedTransactionKind = FindViewById<TextView>(Resource.Id.textViewSelectedTransactionKind);
			var btnTransaction = FindViewById<ImageView>(Resource.Id.btnSelectTransactionKind);
			textViewSelectedTransactionKind.Click += BtnTransaction_Click;
			btnTransaction.Click += BtnTransaction_Click;

			textViewSelectedAdvertsStatus = FindViewById<TextView>(Resource.Id.textViewSelectedAdvertsStatus);
			var btnAdvertsStatus = FindViewById<ImageView>(Resource.Id.btnSelectAdvertsStatus);
			textViewSelectedAdvertsStatus.Click += BtnAdvertsStatus_Click;
			btnAdvertsStatus.Click += BtnAdvertsStatus_Click;

			var btnSelectCategories = FindViewById<ImageView>(Resource.Id.btnSelectCategoryForMainList);
			this.textViewSelectCategories = FindViewById<TextView>(Resource.Id.textViewSelectedCategoryForMainList);
			textViewSelectCategories.Click += BtnSelectCategories_Click;
			btnSelectCategories.Click += BtnSelectCategories_Click;

			this.textViewSelectedSize = FindViewById<TextView>(Resource.Id.textViewSelectedSizes);
			this.btnSize = FindViewById<ImageView>(Resource.Id.btnSize);
			textViewSelectedSize.Click += BtnSize_Click;
			btnSize.Click += BtnSize_Click;


			var btnDistance = FindViewById<ImageView>(Resource.Id.btnDistance);
			this.textViewSelectedDistance = FindViewById<TextView>(Resource.Id.textViewSelectedDistance);
			textViewSelectedDistance.Click += BtnDistance_Click;
			btnDistance.Click += BtnDistance_Click;

			this.textViewSelectedUser = FindViewById<TextView>(Resource.Id.textViewSelectedUser);
			var btnSelectUser = FindViewById<ImageView>(Resource.Id.btnSelectUser);
			textViewSelectedUser.Click += BtnSelectUser_Click;
			btnSelectUser.Click += BtnSelectUser_Click;


			var btnSorting = FindViewById<ImageView>(Resource.Id.btnSorting);
			this.textViewSelectedSorting = FindViewById<TextView>(Resource.Id.textViewSelectedSorting);
			textViewSelectedSorting.Click += BtnSorting_Click;
			btnSorting.Click += BtnSorting_Click;

		}

		private void BtnSorting_Click(object sender, EventArgs e)
		{
			Action<string> actionAfterSelect = (selctedSortingByName) =>
			{
				this.advertisementsSearchModel.SortingBy = selctedSortingByName.GetEnumValueByDisplayName<SortingBy>();
				SetupSelectedSortingByView();
			};
			var sortingByNames = Enum.GetValues(typeof(SortingBy)).GetAllItemsDisplayNames();
			AlertsService.ShowSingleSelectListString(this, sortingByNames.ToArray(), actionAfterSelect, this.advertisementsSearchModel.SortingBy.GetDisplayName(), dialogTitle: "Wybierz rodzaj sortowania");
		}

		private void BtnSelectUser_Click(object sender, EventArgs e)
		{
			var intent = new Intent(this, typeof(FindUserActivity));
			intent.PutExtra(ActivityStateConsts.CALLING_ACTIVITY_NAME, ActivityStateConsts.MAIN_ACTIVITY_NAME);
			StartActivityForResult(intent, FindUserActivity.FIND_USER_REQUEST_CODE);
		}

		private void BtnDistance_Click(object sender, EventArgs e)
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

				this.textViewSelectedDistance.Text = advertisementsSearchModel.CoordinatesModel.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE ? String.Format("{0} km", advertisementsSearchModel.CoordinatesModel.MaxDistance.ToString()) : "Bez ograniczeñ";
			});
		}

		private void BtnSize_Click(object sender, EventArgs e)
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
		}

		private async void BtnSelectCategories_Click(object sender, EventArgs e)
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

			try
			{
				var userSelectesKeywordsNames = this.advertisementsSearchModel.CategoriesModel.Select(c => c.Value).ToList();
				await this.categoriesHelper.ShowCategoriesListAndMakeAction(userSelectesKeywordsNames, methodToExecuteAfterCategoriesSelect);
			}
			catch
			{
			}
		}

		private void BtnAdvertsStatus_Click(object sender, EventArgs e)
		{
			Action<string> action = (status) =>
			{
				this.advertisementsSearchModel.ExpiredAdvertisements = status == expiredStatus ? true : false;
				SetupSelectedAdvertStatus();
			};
			var statuses = new string[] { activeStatus, expiredStatus };
			AlertsService.ShowSingleSelectListString(this, statuses, action, this.textViewSelectedAdvertsStatus.Text);
		}

		private void BtnTransaction_Click(object sender, EventArgs e)
		{
			Action<string> action = (transactionKindName) =>
			{
				var selectedKind = transactionKindName.GetEnumValueByDisplayName<TransactionKind>();
				this.advertisementsSearchModel.TransactionKind = selectedKind;
				SetupSelectedTransactionKind();
			};
			var transactionKindsNames = Enum.GetValues(typeof(TransactionKind)).GetAllItemsDisplayNames();
			AlertsService.ShowSingleSelectListString(this, transactionKindsNames.ToArray(), action, this.textViewSelectedTransactionKind.Text);
		}

		private async Task DownloadAndShowAdvertisements(bool resetList)
		{
			SetAdvertisementListPageNumber(resetList);
			List<AdvertisementItemShort> advertisements = await GetAdvertisements();

			if (advertisements.Count > 0 || resetList)
			{
				if (advertisementItemListAdapter == null || resetList)
				{
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, advertisements, this.advertisementsSearchModel.AdvertisementsKind, this);
					advertisementItemListAdapter.AdvertisementItemClick += AdvertisementItemListAdapter_AdvertisementItemClick;
					advertisementItemListAdapter.DeleteAdvertisementItemClick += AdvertisementItemListAdapter_DeleteAdvertisementItemClick;
					var mLayoutManager = new GridLayoutManager(this, 2);
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

			SetRecyclerVisibility(advertisements);
		}

		private void SetRecyclerVisibility(List<AdvertisementItemShort> advertisements)
		{
			if (advertisements.Count == 0)
			{
				if (advertisementItemListAdapter.ItemCount == 0)
				{
					mainListSwipeLayout.Visibility = ViewStates.Gone;
					textViewNoAdverts.Visibility = ViewStates.Visible;
				}
			}
			else if (mainListSwipeLayout.Visibility == ViewStates.Gone)
			{
				mainListSwipeLayout.Visibility = ViewStates.Visible;
				textViewNoAdverts.Visibility = ViewStates.Gone;
			}
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
						this.advertisementsSearchModel.CoordinatesModel.Longitude = settingsMOdel.LocationSettings.Longitude;
					}
					else
					{
						AlertsService.ShowLongToast(this, "Nie masz ustawionej lokalizacji domowej. Mo¿esz to zrobiæ w lewym panelu");
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

		private void AdvertisementItemListAdapter_DeleteAdvertisementItemClick(object sender, FabOnAdvertisementItemRowClicked clickArgs)
		{
			if (clickArgs.Id == 0)
			{
				AlertsService.ShowLongToast(this, "Wyst¹pi³ b³¹d");
				return;
			}

			var message = clickArgs.Action.GetDisplayName();

			AlertsService.ShowConfirmDialog(this, message, async () =>
			{
				var success = false;
				if (clickArgs.Action == Models.Enums.ActionKindAfterClickFabOnAdvertisementItemRow.Restart)
				{
					success = await this.advertisementItemService.RestartAdvertisement(clickArgs.Id);
				}
				else
				{
					success = await this.advertisementItemService.DeleteAdvertisement(clickArgs.Id, this.advertisementsSearchModel.AdvertisementsKind);
				}

				if (success)
				{
					AlertsService.ShowLongToast(this, "Pomyœlnie zakoñczono tê operacjê.");
					RefreshAdvertisementList(true);
				}
				else
				{
					AlertsService.ShowLongToast(this, "Nie uda³o siê wykonaæ tej operacji.");
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
			this.advertisementsSearchModel.TransactionKind = TransactionKind.All;
			this.advertisementsSearchModel.CategoriesModel.Clear();
			this.advertisementsSearchModel.Sizes.Clear();
			this.advertisementsSearchModel.UserInfo = null;
			this.advertisementsSearchModel.CoordinatesModel.MaxDistance = ValueConsts.MAX_DISTANCE_VALUE;
			this.advertisementsSearchModel.SortingBy = SortingBy.sortByNearest;
			this.advertisementsSearchModel.ExpiredAdvertisements = false;
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


		public void SetupSelectedTransactionKind()
		{
			this.textViewSelectedTransactionKind.Text = this.advertisementsSearchModel.TransactionKind.GetDisplayName();
		}
		public void SetupSelectedAdvertStatus()
		{
			this.textViewSelectedAdvertsStatus.Text = this.advertisementsSearchModel.ExpiredAdvertisements ? expiredStatus : activeStatus;
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
			if (this.advertisementsSearchModel.Sizes.Count > 0)
			{
				optionsSelected = true;
			}
			if (this.advertisementsSearchModel.ExpiredAdvertisements == true)
			{
				optionsSelected = true;
			}
			if (this.advertisementsSearchModel.TransactionKind != TransactionKind.All)
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

		protected override void Toolbar_NavigationClick(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
		{
			OnBackPressed();
		}

		private void FabFilter_Click(object sender, EventArgs e)
		{
			TogleLayouts();
		}

		private void TogleLayouts()
		{
			if (mainListLayout.Visibility == ViewStates.Visible)
			{
				searchModelCopier = new AdvertisementSearchModelCopier(this.advertisementsSearchModel);
				SetAppBarOptions(true);
				SupportActionBar.SetDisplayHomeAsUpEnabled(false);
				SupportActionBar.SetDisplayHomeAsUpEnabled(true);
				drawerToggle.DrawerIndicatorEnabled = false;
				mainListLayout.Visibility = ViewStates.Gone;
				sortingOptionsLayout.Visibility = ViewStates.Visible;
			}
			else
			{
				SetAppBarOptions(false);
				drawerToggle.DrawerIndicatorEnabled = true;
				SetupDrawer();
				sortingOptionsLayout.Visibility = ViewStates.Gone;
				mainListLayout.Visibility = ViewStates.Visible;
			}
		}

		private void Fab_Click(object sender, EventArgs e)
		{
			var addAdvertisementIntent = new Intent(this, typeof(AddNewAdvertisementActivity));
			StartActivity(addAdvertisementIntent);
		}

		public void OnClick(View v)
		{
			OnBackPressed();
		}
	}
}