using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using MobileSecondHand.App.Activities;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Advertisements;

namespace MobileSecondHand.App.Activities{
	[Activity]
	public class AddNewAdvertisementActivity : BaseActivity
	{
		BitmapOperationService bitmapOperationService;
		GpsLocationService gpsLocationService;
		private EditText advertisementDescription;
		private EditText advertisementPrice;
		private EditText advertisementTitle;
		private Button mButtonTakePicture;
		private ImageView mPhotoView1;
		private ProgressDialogHelper progress;
		private RadioButton rdBtnOnlyForSell;
		private string mPhotoPath;
		private bool photoIsTaking;
		private int REQUEST_TAKE_PHOTO = 1;
		private int imageViewDefaultWidth = 300;
		private int imageViewDefaultHeight = 230;
		private string keyAdvertisementTitleText = "advertisementTitleText";
		private string keyRdBtnOnlyForSellValue = "rdBtnOnlyForSellValue";
		private string keyAdvertisementDescriptionText = "advertisementDescriptionText";
		private string keyAdvertisementPriceValue = "advertisementPriceValue";
		private string keyPhotoView1Path = "mPhotoView1Path";
		private string keyPhotoIsTakingValue = "keyPhotoIsTakingValue";
		private Button buttonPublishAdvertisement;
		private View focusView;
		IAdvertisementItemService advertisementItemService;

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			this.bitmapOperationService = new BitmapOperationService();
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.gpsLocationService = new GpsLocationService(this, null);
			SetContentView(Resource.Layout.AddNewAdvertisementActivity);
			base.SetupToolbar();
			SetupViews(savedInstanceState);
			if (savedInstanceState != null) {
				RestoreViewFieldsValues(savedInstanceState);
			}
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == REQUEST_TAKE_PHOTO) {
				SetPhoto();
			}

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

		protected override void OnSaveInstanceState(Bundle outState) {
			base.OnSaveInstanceState(outState);
			SaveViewFieldsValues(outState);
		}

		private void SaveViewFieldsValues(Bundle outState) {
			outState.PutAll(outState);
			outState.PutString(keyAdvertisementTitleText, advertisementTitle.Text);
			outState.PutString(keyAdvertisementDescriptionText, advertisementDescription.Text);
			outState.PutBoolean(keyRdBtnOnlyForSellValue, rdBtnOnlyForSell.Checked);
			outState.PutString(keyAdvertisementPriceValue, advertisementPrice.Text);
			outState.PutString(keyPhotoView1Path, mPhotoPath);
			outState.PutBoolean(keyPhotoIsTakingValue, photoIsTaking);
		}

		private void RestoreViewFieldsValues(Bundle savedInstanceState) {
			advertisementTitle.Text = savedInstanceState.GetString(keyAdvertisementTitleText, String.Empty);
			advertisementDescription.Text = savedInstanceState.GetString(keyAdvertisementDescriptionText, String.Empty);
			advertisementPrice.Text = savedInstanceState.GetString(keyAdvertisementPriceValue, String.Empty);
			rdBtnOnlyForSell.Checked = savedInstanceState.GetBoolean(keyRdBtnOnlyForSellValue, false);
			mPhotoPath = savedInstanceState.GetString(keyPhotoView1Path, null);
			photoIsTaking = savedInstanceState.GetBoolean(keyPhotoIsTakingValue, true);
			if (mPhotoPath != null && !photoIsTaking) {
				SetPhoto();
			}
		}


		private void SetPhoto() {
			photoIsTaking = true;
			int targetW = mPhotoView1.Width;
			int targetH = mPhotoView1.Height;
			Bitmap resizedImage = this.bitmapOperationService.GetBitmap(mPhotoPath);
			mPhotoView1.SetImageBitmap(resizedImage);
			mButtonTakePicture.Text = "Zrób inne zdjêcie";
			photoIsTaking = false;
		}

		private void SetupViews(Bundle savedInstanceState) {
			rdBtnOnlyForSell = (RadioButton)FindViewById(Resource.Id.rdBtnOnlyForSell);
			progress = new ProgressDialogHelper(this);
			advertisementTitle = (EditText)FindViewById(Resource.Id.editTextTitle);
			advertisementDescription = (EditText)FindViewById(Resource.Id.editTextDescription);
			advertisementPrice = (EditText)FindViewById(Resource.Id.editTextPrice);
			mPhotoView1 = (ImageView)FindViewById(Resource.Id.photoView1);
			mButtonTakePicture = (Button)FindViewById(Resource.Id.buttonTakePicture);
			buttonPublishAdvertisement = FindViewById<Button>(Resource.Id.buttonPublishAdvertisemenetItem);

			buttonPublishAdvertisement.Click += async (s,e) => await ButtonPublishAdvertisement_Click(s, e);
			mButtonTakePicture.Click += MButtonTakePicture_Click;
		}

		private async Task ButtonPublishAdvertisement_Click(object sender, EventArgs e) {
			progress.ShowProgressDialog("Wysy³anie ogloszenia. Proszê czekaæ...");
			if (AdvertisementItemDataIsValidate()) {
				var location = gpsLocationService.GetLocation();
				if (location.Longitude != 0.0 && location.Latitude != 0.0) {
					var bytesArray = System.IO.File.ReadAllBytes(mPhotoPath);
					var resized = this.bitmapOperationService.ResizeImageAndGetByteArray(bytesArray);
					var photosListModel = await this.advertisementItemService.UploadNewAdvertisementPhotos(resized);
					if (photosListModel != null) {
						var newAdvertisementModel = CreateNewAdvertisementItemModel(photosListModel);
						var success = await this.advertisementItemService.CreateNewAdvertisement(newAdvertisementModel);
						if (success) {
							AlertsService.ShowToast(this, "Pomyœlnie utworzone nowe og³oszenie");
							this.Finish();
						}
						else {
							AlertsService.ShowToast(this, "Nie uda³o siê utworzyæ nowego og³oszenia. Spróbuj ponownie");
						}

					}
				}
				else {
					//lokalizacja jest chujowa
					Toast.MakeText(this, "Wspólrzêdne lokalizacji s¹ zerowe", ToastLength.Long).Show();
				}
			}
			progress.CloseProgressDialog();
		}

		private NewAdvertisementItem CreateNewAdvertisementItemModel(AdvertisementItemPhotosPaths photosListModel) {
			var location = this.gpsLocationService.GetLocation();
			NewAdvertisementItem model = new NewAdvertisementItem();
			model.AdvertisementTitle = advertisementTitle.Text;
			model.AdvertisementDescription = advertisementDescription.Text;
			model.Latitude = location.Latitude;
			model.Longitude = location.Longitude;
			model.IsOnlyForSell = rdBtnOnlyForSell.Checked;
			model.AdvertisementPrice = Int32.Parse(advertisementPrice.Text);
			model.PhotosPaths = photosListModel.PhotosPaths;

			return model;
		}

		private bool AdvertisementItemDataIsValidate() {
			bool isValidate = true;
			if (advertisementTitle.Text.Length < 5) {
				isValidate = false;
				advertisementTitle.Error = "Tytu³ musi zawieraæ min. 15 znaków";
				focusView = advertisementTitle;
			}
			else if (advertisementDescription.Text.Length < 5) {
				isValidate = false;
				advertisementDescription.Error = "Opis musi zawieraæ min 20 znaków";
				focusView = advertisementDescription;
			}
			else if (advertisementPrice.Text.Length == 0) {
				isValidate = false;
				advertisementPrice.Error = "Cena musi zostaæ podana";
				focusView = advertisementPrice;
			}
			else if (mPhotoPath == null) {
				Toast.MakeText(this, "Nie dodano ¿adnego zdjêcia", ToastLength.Long).Show();
				isValidate = false;
			}
			return isValidate;
		}

		private void MButtonTakePicture_Click(object sender, EventArgs e) {
			Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
			if (takePictureIntent.ResolveActivity(PackageManager) != null) {
				Java.IO.File photoFile = null;
				try {
					photoFile = CreateImageFile();
				} catch (Java.IO.IOException ex) {
					Toast.MakeText(this, "Coœ sie zjeba³o ze zdjeciem", ToastLength.Long).Show();
				}
				if (photoFile != null) {
					photoIsTaking = true;
					takePictureIntent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(photoFile));
					StartActivityForResult(takePictureIntent, REQUEST_TAKE_PHOTO);
				}
			}
		}

		private Java.IO.File CreateImageFile() {
			String timeStamp = DateTime.Now.ToString();
			String imageFileName = "JPEG_" + timeStamp + "_";
			Java.IO.File storageDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);

			Java.IO.File image = Java.IO.File.CreateTempFile(
					imageFileName,  /* prefix */
					".jpg",         /* suffix */
					storageDir      /* directory */
			);

			mPhotoPath = image.AbsolutePath;
			return image;
		}
	}
}