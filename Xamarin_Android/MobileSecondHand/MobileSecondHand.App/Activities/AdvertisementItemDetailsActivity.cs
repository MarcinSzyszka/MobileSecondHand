using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Chat;
using Newtonsoft.Json;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Szczegó³y og³oszenia")]
	public class AdvertisementItemDetailsActivity : BaseActivity
	{
		private ProgressDialogHelper progress;
		IAdvertisementItemService advertisementItemService;
		IMessagesService messagesService;
		BitmapOperationService bitmapOperationService;
		SharedPreferencesHelper sharedPreferencesHelper;
		private TextView sellerNetworkStateInfoTextView;
		private TextView forSellOrChangeInfoTextView;
		private TextView price;
		private TextView title;
		private TextView description;
		private Button showOtherAdvertisementsBtn;
		private Button startConversationBtn;
		private ImageView photoView;
		private TextView distanceTextView;
		private int photoImageViewWitdth;
		private int photoImageViewHeight;
		private AdvertisementItemDetails advertisement;

		public AdvertisementItemDetailsActivity() {
			this.advertisementItemService = new AdvertisementItemService();
			this.bitmapOperationService = new BitmapOperationService();
			messagesService = new MessagesService();
		}
		protected override async void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(this);
			SetContentView(Resource.Layout.AdvertisementItemDetailsActivity);
			base.SetupToolbar();
			SetupViews();
			CalculateSizeForPhotoImageView();
			await GetAndShowAdvertisementDetails();
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
		private void CalculateSizeForPhotoImageView() {
			var metrics = this.Resources.DisplayMetrics;
			var width = metrics.WidthPixels - 20;
			this.photoImageViewWitdth = width;
			this.photoImageViewHeight = (int)(width * 0.8);
		}

		private void SetupViews() {
			this.progress = new ProgressDialogHelper(this);
			this.distanceTextView = FindViewById<TextView>(Resource.Id.distanceDetailsTextView);
			this.sellerNetworkStateInfoTextView = FindViewById<TextView>(Resource.Id.sellerNetworkState);
			this.forSellOrChangeInfoTextView = FindViewById<TextView>(Resource.Id.forSellOrChangeInfo);
			this.startConversationBtn = FindViewById<Button>(Resource.Id.startConvesationBtn);
			this.photoView = FindViewById<ImageView>(Resource.Id.advertisementDetailsPhoto);
			this.price = FindViewById<TextView>(Resource.Id.advertisementDeatilsPrice);
			this.title = FindViewById<TextView>(Resource.Id.advertisementDetailsTitle);
			this.description = FindViewById<TextView>(Resource.Id.advertisementDetailsDescription);
			this.showOtherAdvertisementsBtn = FindViewById<Button>(Resource.Id.showOtherUserAdvertisementsBtn);
		}

		private async Task GetAndShowAdvertisementDetails() {
			progress.ShowProgressDialog("Pobieranie szczegó³ów og³oszenia...");
			var advertisementItemId = Intent.GetIntExtra(ExtrasKeys.ADVERTISEMENT_ITEM_ID, 0);
			advertisement = await GetAdvertisement(advertisementItemId);
			ShowAdvertisementDetails(advertisement, Intent.GetDoubleExtra(ExtrasKeys.ADVERTISEMENT_ITEM_DISTANCE, 0.0));
			progress.CloseProgressDialog();
		}

		private void ShowAdvertisementDetails(AdvertisementItemDetails advertisement, double distance) {
			//distanceTextView.Text = String.Format("{0}{1} km", this.Resources.GetString(Resource.String.distanceDetailsInfo), distance);
			distanceTextView.Text = String.Format("{0} km", distance);
			if (advertisement.IsSellerOnline) {
				sellerNetworkStateInfoTextView.Text = this.Resources.GetString(Resource.String.userOnlineStateInfo);
				sellerNetworkStateInfoTextView.SetTextColor(Android.Graphics.Color.Green);
			}
			else {
				sellerNetworkStateInfoTextView.Text = this.Resources.GetString(Resource.String.userOfflineStateInfo);
				sellerNetworkStateInfoTextView.SetTextColor(Android.Graphics.Color.Red);
			}

			forSellOrChangeInfoTextView.Text = advertisement.IsOnlyForSell ?
												this.Resources.GetString(Resource.String.onlyForSellInfo) :
												this.Resources.GetString(Resource.String.forSellOrChangeInfo);
			photoView.LayoutParameters.Width = photoImageViewWitdth;
			photoView.LayoutParameters.Height = photoImageViewHeight;
			photoView.RequestLayout();
			photoView.SetImageBitmap(bitmapOperationService.ResizeImage(advertisement.Photo, photoImageViewWitdth, photoImageViewHeight));
			price.Text = String.Format("{0} z³", advertisement.Price);
			title.Text = advertisement.Title;
			description.Text = advertisement.Description;
			startConversationBtn.Click += async (s, e) => await StartConversationBtn_Click(s, e);
			showOtherAdvertisementsBtn.Click += ShowOtherAdvertisementsBtn_Click;

		}

		private void ShowOtherAdvertisementsBtn_Click(object sender, EventArgs e) {
			throw new NotImplementedException();
		}

		private async Task StartConversationBtn_Click(object sender, EventArgs e) {
			progress.ShowProgressDialog("Proszê czekaæ. Trwa przetwarzanie informacji..");
			var bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			var conversationInfoModel = await messagesService.GetConversationInfoModel(this.advertisement.SellerId, bearerToken);
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
				//conversationIntent.PutExtra(ExtrasKeys.CONVERSATION_ID, conversationInfoModel);
				//conversationIntent.PutExtra(ExtrasKeys.ADDRESSEE_ID, advertisement.SellerId);
				StartActivity(conversationIntent);
			}
		}

		private async Task<AdvertisementItemDetails> GetAdvertisement(int advertisementItemId) {
			var tokenModel = new TokenModel();
			tokenModel.Token = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			var advertisement = await this.advertisementItemService.GetAdvertisementDetails(advertisementItemId, tokenModel);

			return advertisement;
		}
	}
}