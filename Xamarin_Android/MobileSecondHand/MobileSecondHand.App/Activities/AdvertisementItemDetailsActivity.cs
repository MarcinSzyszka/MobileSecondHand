using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using ImageViews.Photo;
using MobileSecondHand.API.Models.Shared;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Chat;
using Newtonsoft.Json;

namespace MobileSecondHand.App.Activities
{
	[Activity]
	public class AdvertisementItemDetailsActivity : BaseActivity
	{
		private ProgressDialogHelper progress;
		IAdvertisementItemService advertisementItemService;
		IMessagesService messagesService;
		BitmapOperationService bitmapOperationService;
		private TextView sellerNetworkStateInfoTextView;
		private TextView forSellOrChangeInfoTextView;
		private TextView price;
		private TextView title;
		private TextView description;
		private Button showOtherAdvertisementsBtn;
		private Button startConversationBtn;
		private Button addToFavouriteAdvertsBtn;
		private ImageView photoView1;
		private ImageView photoView2;
		private ImageView photoView3;
		private TextView distanceTextView;
		private AdvertisementItemDetails advertisement;
		private RelativeLayout advertisementDetailsWrapperLayout;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			messagesService = new MessagesService(bearerToken);
			this.bitmapOperationService = new BitmapOperationService();

			SetContentView(Resource.Layout.AdvertisementItemDetailsActivity);
			base.SetupToolbar();
			SetupViews();
			await GetAndShowAdvertisementDetails();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			SaveViewFieldsValues(outState);
		}

		private void SaveViewFieldsValues(Bundle outState)
		{
			//TODO zrobic zapamietywanie danyc przy zmianie orientacji zeby nie ciagnac znowu danych
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.conversationMenu, menu);
			if (menu != null)
			{
				menu.FindItem(Resource.Id.home).SetVisible(true);
				menu.FindItem(Resource.Id.chat).SetVisible(true);
			}

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var handled = false;
			switch (item.ItemId)
			{
				case Resource.Id.home:
					GoToMainPage();
					handled = true;
					break;
				case Resource.Id.chat:
					GoToChat();
					handled = true;
					break;
			}

			return handled;
		}

		private void SetupViews()
		{
			this.progress = new ProgressDialogHelper(this);
			this.advertisementDetailsWrapperLayout = FindViewById<RelativeLayout>(Resource.Id.advertisementDetailsWrapperLayout);
			this.distanceTextView = FindViewById<TextView>(Resource.Id.distanceDetailsTextView);
			this.sellerNetworkStateInfoTextView = FindViewById<TextView>(Resource.Id.sellerNetworkState);
			this.forSellOrChangeInfoTextView = FindViewById<TextView>(Resource.Id.forSellOrChangeInfo);
			this.startConversationBtn = FindViewById<Button>(Resource.Id.startConvesationBtn);
			this.photoView1 = FindViewById<ImageView>(Resource.Id.advertisementDetailsPhoto1);
			this.photoView2 = FindViewById<ImageView>(Resource.Id.advertisementDetailsPhoto2);
			this.photoView3 = FindViewById<ImageView>(Resource.Id.advertisementDetailsPhoto3);
			this.price = FindViewById<TextView>(Resource.Id.advertisementDeatilsPrice);
			this.title = FindViewById<TextView>(Resource.Id.advertisementDetailsTitle);
			this.description = FindViewById<TextView>(Resource.Id.advertisementDetailsDescription);
			this.showOtherAdvertisementsBtn = FindViewById<Button>(Resource.Id.showOtherUserAdvertisementsBtn);
			this.showOtherAdvertisementsBtn.Visibility = ViewStates.Invisible;
			this.addToFavouriteAdvertsBtn = FindViewById<Button>(Resource.Id.btnAddToFavoriteAdvertisements);
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

		private void ShowAdvertisementDetails(AdvertisementItemDetails advertisement, double distance)
		{
			//distanceTextView.Text = String.Format("{0}{1} km", this.Resources.GetString(Resource.String.distanceDetailsInfo), distance);
			distanceTextView.Text = String.Format("{0} km", distance);
			if (advertisement.IsSellerOnline)
			{
				sellerNetworkStateInfoTextView.Text = this.Resources.GetString(Resource.String.userOnlineStateInfo);
				sellerNetworkStateInfoTextView.SetTextColor(Android.Graphics.Color.Green);
			}
			else
			{
				sellerNetworkStateInfoTextView.Text = this.Resources.GetString(Resource.String.userOfflineStateInfo);
				sellerNetworkStateInfoTextView.SetTextColor(Android.Graphics.Color.Red);
			}

			forSellOrChangeInfoTextView.Text = advertisement.IsOnlyForSell ?
												this.Resources.GetString(Resource.String.onlyForSellInfo) :
												this.Resources.GetString(Resource.String.forSellOrChangeInfo);

			SetPhotots(advertisement);
			price.Text = String.Format("{0} z³", advertisement.Price);
			title.Text = advertisement.Title;
			description.Text = advertisement.Description;
			startConversationBtn.Click += async (s, e) => await StartConversationBtn_Click(s, e);
			this.addToFavouriteAdvertsBtn.Click += async (s, e) => await AddToFavouriteAdvertsBtn_Click(s, e);
			showOtherAdvertisementsBtn.Click += ShowOtherAdvertisementsBtn_Click;

		}

		private void SetPhotots(AdvertisementItemDetails advertisement)
		{
			for (int i = 0; i < advertisement.Photos.Count; i++)
			{
				ImageView currentPhotoView;
				if (i == 0)
				{
				
					currentPhotoView = photoView1;
				}
				else if (i == 1)
				{
					currentPhotoView = photoView2;
				}
				else
				{
					currentPhotoView = photoView3;
				}

				currentPhotoView.SetImageBitmap(bitmapOperationService.GetBitmap(advertisement.Photos[i]));
				//var attacher = new PhotoViewAttacher(currentPhotoView);
				
				currentPhotoView.Visibility = ViewStates.Visible;
			}
		}

		private void ShowOtherAdvertisementsBtn_Click(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private async Task AddToFavouriteAdvertsBtn_Click(object sender, EventArgs e)
		{
			var idApiModel = new SingleIdModelForPostRequests { Id = this.advertisement.Id };
			var responseMessage = await this.advertisementItemService.AddToUserFavouritesAdvertisements(idApiModel);
			AlertsService.ShowToast(this, responseMessage);
		}

		private async Task StartConversationBtn_Click(object sender, EventArgs e)
		{
			progress.ShowProgressDialog("Proszê czekaæ. Trwa przetwarzanie informacji..");
			var conversationInfoModel = await messagesService.GetConversationInfoModel(this.advertisement.SellerId);
			progress.CloseProgressDialog();
			if (conversationInfoModel.ConversationId == 0)
			{
				//if 0 that means user is trying to send message to himself
				AlertsService.ShowToast(this, "Nie mo¿esz wys³aæ wiadomoœci do samego siebie :)");
			}
			else
			{
				var conversationIntent = new Intent(this, typeof(ConversationActivity));
				conversationIntent.PutExtra(ExtrasKeys.CONVERSATION_INFO_MODEL, JsonConvert.SerializeObject(conversationInfoModel));
				StartActivity(conversationIntent);
			}
		}

		private async Task<AdvertisementItemDetails> GetAdvertisement(int advertisementItemId)
		{
			var advertisement = await this.advertisementItemService.GetAdvertisementDetails(advertisementItemId);

			return advertisement;
		}
	}
}