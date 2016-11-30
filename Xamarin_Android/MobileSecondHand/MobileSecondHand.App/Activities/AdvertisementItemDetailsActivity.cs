using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.API.Models.Shared;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Chat;
using MobileSecondHand.Services.Feedback;
using Newtonsoft.Json;
using Refractored.Controls;
using MobileSecondHand.API.Models.Shared.Extensions;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Szczegó³y", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class AdvertisementItemDetailsActivity : BaseActivity, IInfiniteScrollListener
	{
		RecyclerView advertisementsRecyclerView;
		AdvertisementItemListAdapter advertisementItemListAdapter;
		private ProgressDialogHelper progress;
		IAdvertisementItemService advertisementItemService;
		IFeedbackService feedbackService;
		IMessagesService messagesService;
		BitmapOperationService bitmapOperationService;
		private ImageView sellerNetworkStateInfoImageView;
		private TextView forSellOrChangeInfoTextView;
		private TextView price;
		private TextView title;
		private TextView description;
		private TextView sellerName;
		TextView textViewSizeValue;
		TextView textViewAdvertStatus;
		private ImageView startConversationBtn;
		private TextView distanceTextView;
		CircleImageView userPhoto;
		private AdvertisementItemDetails advertisement;
		private RelativeLayout advertisementDetailsWrapperLayout;
		private int userAdvertsPageNumber;
		private NestedScrollView nestedScrollViewLayout;
		private RelativeLayout userAdvertsLayout;
		private bool firstEntryOnUserAdvertisementsList;
		private RecyclerView photosRecyclerView;
		private IMenu menu;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			feedbackService = new FeedbackService(bearerToken);
			messagesService = new MessagesService(bearerToken);
			this.bitmapOperationService = new BitmapOperationService();

			SetContentView(Resource.Layout.AdvertisementItemDetailsActivity);
			base.SetupToolbar();
			SetupViews();
			await GetAndShowAdvertisementDetails();
			firstEntryOnUserAdvertisementsList = true;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.advertisementDeatilsMenu, menu);
			if (menu != null)
			{
				this.menu = menu;
				SetAppBarMenuVisibility(true);
			}

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var handled = false;
			switch (item.ItemId)
			{
				case Resource.Id.addToFavourites:
					AddToFavourites();
					handled = true;
					break;
				case Resource.Id.report:
					ReportWrongAdvert();
					handled = true;
					break;
				case Resource.Id.moreUserAdverts:
					TogleLayouts();
					handled = true;
					break;


			}

			return handled;
		}



		protected override void Toolbar_NavigationClick(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
		{
			OnBackPressed();
		}

		private void SetupViews()
		{
			this.progress = new ProgressDialogHelper(this);
			this.advertisementDetailsWrapperLayout = FindViewById<RelativeLayout>(Resource.Id.advertisementDetailsWrapperLayout);
			this.distanceTextView = FindViewById<TextView>(Resource.Id.distanceDetailsTextView);
			this.sellerNetworkStateInfoImageView = FindViewById<ImageView>(Resource.Id.sellerNetworkState);
			this.forSellOrChangeInfoTextView = FindViewById<TextView>(Resource.Id.forSellOrChangeInfo);
			this.startConversationBtn = FindViewById<ImageView>(Resource.Id.startConvesationBtn);
			this.price = FindViewById<TextView>(Resource.Id.advertisementDeatilsPrice);
			this.title = FindViewById<TextView>(Resource.Id.advertisementDetailsTitle);
			this.description = FindViewById<TextView>(Resource.Id.advertisementDetailsDescription);
			this.sellerName = FindViewById<TextView>(Resource.Id.textViewUserNameAdvertDetails);
			this.textViewAdvertStatus = FindViewById<TextView>(Resource.Id.textViewAdvertStatus);
			this.userPhoto = FindViewById<CircleImageView>(Resource.Id.profile_image_on_advert_det);
			this.userPhoto.Click += (s, e) => TogleLayouts();
			this.nestedScrollViewLayout = FindViewById<NestedScrollView>(Resource.Id.nestedScrollViewLayout);
			this.userAdvertsLayout = FindViewById<RelativeLayout>(Resource.Id.userAdvertisementsRecyclerViewWrapper);
			textViewSizeValue = FindViewById<TextView>(Resource.Id.textViewSizeValue);
			advertisementsRecyclerView = FindViewById<RecyclerView>(Resource.Id.advertisementsRecyclerViewOnAdvertDetails);
			var mLayoutManager = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
			advertisementsRecyclerView.SetLayoutManager(mLayoutManager);

			photosRecyclerView = FindViewById<RecyclerView>(Resource.Id.photosRecyclerViewOnAdvertDetails);
			var photoRecyclerLayoutManager = new LinearLayoutManager(this);
			photoRecyclerLayoutManager.Orientation = LinearLayoutManager.Horizontal;
			photosRecyclerView.SetLayoutManager(photoRecyclerLayoutManager);

			this.nestedScrollViewLayout.RequestLayout();
		}

		public override void OnBackPressed()
		{
			if (userAdvertsLayout.Visibility == ViewStates.Visible)
			{
				TogleLayouts();
			}
			else
			{
				base.OnBackPressed();
			}
		}

		private void ReportWrongAdvert()
		{
			Action reportActionConfirmed = () =>
			{
				Action<string> actionOnReasonSelected = async (reason) =>
				{
					this.progress.ShowProgressDialog("Zg³aszanie og³oszenia naruszaj¹cego regulamin");
					await this.feedbackService.ReportWrongAdvertisement(this.advertisement.Id, reason);
					this.progress.CloseProgressDialog();
					AlertsService.ShowShortToast(this, "Og³oszenie zosta³o zg³oszone adminom");
				};
				string[] itemList = Resources.GetStringArray(Resource.Array.report_wrong_advert_reasons);
				AlertsService.ShowSingleSelectListString(this, itemList, actionOnReasonSelected, dialogTitle: "Wybierz powód");
			};
			AlertsService.ShowConfirmDialog(this, "Czy na pewno chcesz zg³osiæ to og³oszenie jako naruszenie regulaminu?", reportActionConfirmed);
		}

		private async void TogleLayouts()
		{
			if (nestedScrollViewLayout.Visibility == ViewStates.Visible)
			{
				nestedScrollViewLayout.Visibility = ViewStates.Gone;
				SetAppBarMenuVisibility(false);
				this.toolbar.Title = String.Format("Og³oszenia: {0}", advertisement.SellerName);
				userAdvertsLayout.Visibility = ViewStates.Visible;
				if (firstEntryOnUserAdvertisementsList)
				{
					await DownloadAndShowAdvertisements();
					firstEntryOnUserAdvertisementsList = false;
				}
			}
			else
			{
				this.toolbar.Title = "Szczegó³y";
				SetAppBarMenuVisibility(true);
				nestedScrollViewLayout.Visibility = ViewStates.Visible;
				userAdvertsLayout.Visibility = ViewStates.Gone;
			}
		}

		private void SetAppBarMenuVisibility(bool isVisible)
		{
			menu.FindItem(Resource.Id.addToFavourites).SetVisible(isVisible);
			menu.FindItem(Resource.Id.report).SetVisible(isVisible);
			menu.FindItem(Resource.Id.moreUserAdverts).SetVisible(isVisible);
		}

		private async Task DownloadAndShowAdvertisements()
		{
			progress.ShowProgressDialog("Pobieranie og³oszeñ. Proszê czekaæ...");
			List<AdvertisementItemShort> advertisements = await this.advertisementItemService.GetUserAdvertisements(userAdvertsPageNumber, this.advertisement.SellerId);

			if (advertisements.Count > 0)
			{
				if (advertisementItemListAdapter == null)
				{
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, advertisements, AdvertisementsKind.AdvertisementsAroundUserCurrentLocation, this);
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
					advertisementItemListAdapter = new AdvertisementItemListAdapter(this, new List<AdvertisementItemShort>(), AdvertisementsKind.AdvertisementsAroundUserCurrentLocation, this);
				}
				advertisementItemListAdapter.InfiniteScrollDisabled = true;
				userAdvertsPageNumber = 0;
			}
			progress.CloseProgressDialog();
		}


		private void AdvertisementItemListAdapter_AdvertisementItemClick(object sender, ShowAdvertisementDetailsEventArgs e)
		{
			//przejœcie do widoku detalue dokumentu
			var intent = new Intent(this, typeof(AdvertisementItemDetailsActivity));
			intent.PutExtra(ExtrasKeys.ADVERTISEMENT_ITEM_ID, e.Id);
			intent.PutExtra(ExtrasKeys.ADVERTISEMENT_ITEM_DISTANCE, e.Distance);
			StartActivity(intent);
		}

		private async Task GetAndShowAdvertisementDetails()
		{
			progress.ShowProgressDialog("Pobieranie szczegó³ów og³oszenia...");
			this.advertisementDetailsWrapperLayout.Visibility = ViewStates.Invisible;
			var advertisementItemId = Intent.GetIntExtra(ExtrasKeys.ADVERTISEMENT_ITEM_ID, 0);
			advertisement = await GetAdvertisement(advertisementItemId);
			this.advertisementDetailsWrapperLayout.Visibility = ViewStates.Visible;
			ShowAdvertisementDetails(advertisement, Intent.GetDoubleExtra(ExtrasKeys.ADVERTISEMENT_ITEM_DISTANCE, 0.0));
			progress.CloseProgressDialog();
		}

		private async void ShowAdvertisementDetails(AdvertisementItemDetails advertisement, double distance)
		{
			distanceTextView.Text = String.Format("{0} km", distance);
			textViewSizeValue.Text = advertisement.Size.GetDisplayName();
			if (advertisement.IsSellerOnline)
			{
				sellerNetworkStateInfoImageView.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.userOnline));
			}
			else
			{
				sellerNetworkStateInfoImageView.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.userffline));
			}

			forSellOrChangeInfoTextView.Text = advertisement.IsOnlyForSell ?
												this.Resources.GetString(Resource.String.onlyForSellInfo) :
												this.Resources.GetString(Resource.String.forSellOrChangeInfo);

			var photosAdapter = new AdvertisementPhotosListAdapter(this.advertisement.Photos);
			photosAdapter.PhotoClicked += PhotosAdapter_PhotoClicked;
			photosRecyclerView.SetAdapter(photosAdapter);
			price.Text = String.Format("{0} z³", advertisement.Price);
			title.Text = advertisement.Title;
			description.Text = advertisement.Description;
			sellerName.Text = advertisement.SellerName;
			if (advertisement.SellerProfileImage != null)
			{
				userPhoto.SetImageBitmap(await this.bitmapOperationService.GetScaledDownBitmapForDisplayAsync(advertisement.SellerProfileImage));
			}
			textViewAdvertStatus.Text = String.Format("Og³oszenie {0} {1}", advertisement.IsActive ? "aktywne do" : "zakoñczone ", advertisement.ExpirationDate.ToShortDateString());
			startConversationBtn.Click += async (s, e) => await StartConversationBtn_Click(s, e);
		}

		private void AddToFavourites()
		{
			AlertsService.ShowConfirmDialog(this, "Czy na pewno dodaæ to og³oszenie do ulubionych?", async () =>
			{
				var idApiModel = new SingleIdModelForPostRequests { Id = this.advertisement.Id };
				var responseMessage = await this.advertisementItemService.AddToUserFavouritesAdvertisements(idApiModel);
				AlertsService.ShowLongToast(this, responseMessage);
			});
		}

		private void PhotosAdapter_PhotoClicked(object sender, int photoIndex)
		{
			var photosViewerIntent = new Intent(this, typeof(PhotosViewerActivity));
			SharedObject.Data = this.advertisement.Photos;
			photosViewerIntent.PutExtra(ActivityStateConsts.SELECTED_PHOTO_INDEX_TO_START_ON_PHOTOS_VIEWER, photoIndex);

			StartActivity(photosViewerIntent);
		}

		private async Task StartConversationBtn_Click(object sender, EventArgs e)
		{
			if (!advertisement.IsActive)
			{
				AlertsService.ShowShortToast(this, "Og³oszenie jest nieaktualne dlatego nie mo¿esz wys³aæ wiadomoœci autorowi og³oszenia");
			}
			else
			{
				progress.ShowProgressDialog("Proszê czekaæ. Trwa przetwarzanie informacji..");
				var conversationInfoModel = await messagesService.GetConversationInfoModel(this.advertisement.SellerId);
				progress.CloseProgressDialog();
				if (conversationInfoModel.ConversationId == 0)
				{
					//if 0 that means user is trying to send message to himself
					AlertsService.ShowLongToast(this, "Nie mo¿esz wys³aæ wiadomoœci do samego siebie :)");
				}
				else
				{
					var conversationIntent = new Intent(this, typeof(ConversationActivity));
					conversationIntent.PutExtra(ExtrasKeys.CONVERSATION_INFO_MODEL, JsonConvert.SerializeObject(conversationInfoModel));
					StartActivity(conversationIntent);
				}
			}
		}

		private async Task<AdvertisementItemDetails> GetAdvertisement(int advertisementItemId)
		{
			var advertisement = await this.advertisementItemService.GetAdvertisementDetails(advertisementItemId);

			return advertisement;
		}

		public async void OnInfiniteScroll()
		{
			userAdvertsPageNumber++;
			await DownloadAndShowAdvertisements();
		}
	}
}