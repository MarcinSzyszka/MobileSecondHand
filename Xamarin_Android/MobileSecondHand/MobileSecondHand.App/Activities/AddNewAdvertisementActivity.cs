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
using MobileSecondHand.Common.Enumerations;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Common.Extensions;

namespace MobileSecondHand.App.Activities
{
	[Activity]
	public class AddNewAdvertisementActivity : BaseActivity
	{
		BitmapOperationService bitmapOperationService;
		GpsLocationService gpsLocationService;
		private EditText advertisementDescription;
		private EditText advertisementPrice;
		private EditText advertisementTitle;
		private ProgressDialogHelper progress;
		private RadioButton rdBtnOnlyForSell;
		private List<string> photosPaths;
		private bool photoIsTaking;
		private int REQUEST_TAKE_PHOTO_1 = 1;
		private int REQUEST_TAKE_PHOTO_2 = 2;
		private int REQUEST_TAKE_PHOTO_3 = 3;
		private string keyAdvertisementTitleText = "advertisementTitleText";
		private string keyRdBtnOnlyForSellValue = "rdBtnOnlyForSellValue";
		private string keyAdvertisementDescriptionText = "advertisementDescriptionText";
		private string keyAdvertisementPriceValue = "advertisementPriceValue";
		private string keyPhotosPaths = "mPhotoView1Path";
		private string keyPhotoIsTakingValue = "keyPhotoIsTakingValue";
		private Button buttonPublishAdvertisement;
		private View focusView;
		IAdvertisementItemService advertisementItemService;
		private ImageView mPhotoView1;
		private Button mButtonTakePicture1;
		private ImageView mPhotoView2;
		private Button mButtonTakePicture2;
		private ImageView mPhotoView3;
		private Button mButtonTakePicture3;
		private List<string> tempPhotosPaths;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.bitmapOperationService = new BitmapOperationService();
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.gpsLocationService = new GpsLocationService(this, null);
			SetContentView(Resource.Layout.AddNewAdvertisementActivity);
			base.SetupToolbar();
			SetupViews(savedInstanceState);
			photosPaths = new List<string>();
			tempPhotosPaths = new List<string>();

			if (savedInstanceState != null)
			{
				RestoreViewFieldsValues(savedInstanceState);
			}
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == Result.Ok && (requestCode == REQUEST_TAKE_PHOTO_1 || requestCode == REQUEST_TAKE_PHOTO_2 || requestCode == REQUEST_TAKE_PHOTO_3))
			{
				if (data != null)
				{
					var file = CreateImageFile(requestCode);
					tempPhotosPaths.Add(this.bitmapOperationService.SavePhotoFromUriAndReturnPhysicalPath(data.Data, file, this));
				}

				SetPhoto(requestCode);
			}
			else
			{
				photoIsTaking = false;
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

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			SaveViewFieldsValues(outState);
		}

		private void SaveViewFieldsValues(Bundle outState)
		{
			outState.PutAll(outState);
			outState.PutString(keyAdvertisementTitleText, advertisementTitle.Text);
			outState.PutString(keyAdvertisementDescriptionText, advertisementDescription.Text);
			outState.PutBoolean(keyRdBtnOnlyForSellValue, rdBtnOnlyForSell.Checked);
			outState.PutString(keyAdvertisementPriceValue, advertisementPrice.Text);
			outState.PutStringArray(keyPhotosPaths, this.photosPaths.ToArray());
			outState.PutBoolean(keyPhotoIsTakingValue, photoIsTaking);

			if (!photoIsTaking)
			{
				this.mPhotoView1.Dispose();
				this.mPhotoView2.Dispose();
				this.mPhotoView3.Dispose();
			}

		}

		private void RestoreViewFieldsValues(Bundle savedInstanceState)
		{
			advertisementTitle.Text = savedInstanceState.GetString(keyAdvertisementTitleText, String.Empty);
			advertisementDescription.Text = savedInstanceState.GetString(keyAdvertisementDescriptionText, String.Empty);
			advertisementPrice.Text = savedInstanceState.GetString(keyAdvertisementPriceValue, String.Empty);
			rdBtnOnlyForSell.Checked = savedInstanceState.GetBoolean(keyRdBtnOnlyForSellValue, false);
			this.photosPaths = savedInstanceState.GetStringArray(keyPhotosPaths).ToList();
			photoIsTaking = savedInstanceState.GetBoolean(keyPhotoIsTakingValue, true);
			var i = 0;
			if (this.photosPaths.Count > 0 && !photoIsTaking)
			{
				do
				{
					i++;
					SetPhoto(i);
				} while (i < this.photosPaths.Count);
			}

		}


		private void SetPhoto(int photoNr)
		{
			Bitmap resizedImage = this.bitmapOperationService.ResizeImageAndGetBitMap(this.photosPaths[photoNr - 1]);
			switch (photoNr)
			{
				case 1:
					{
						mPhotoView1.SetImageBitmap(resizedImage);
						mButtonTakePicture1.Text = "Zr�b inne zdj�cie";
						mButtonTakePicture2.Enabled = true;
						break;
					}
				case 2:
					{
						mPhotoView2.SetImageBitmap(resizedImage);
						mButtonTakePicture2.Text = "Zr�b inne zdj�cie";
						mButtonTakePicture3.Enabled = true;
						break;
					}
				case 3:
					{
						mPhotoView3.SetImageBitmap(resizedImage);
						mButtonTakePicture3.Text = "Zr�b inne zdj�cie";
						break;
					}
				default:
					break;
			}

			photoIsTaking = false;
		}

		private void SetupViews(Bundle savedInstanceState)
		{
			rdBtnOnlyForSell = (RadioButton)FindViewById(Resource.Id.rdBtnOnlyForSell);
			progress = new ProgressDialogHelper(this);
			advertisementTitle = (EditText)FindViewById(Resource.Id.editTextTitle);
			advertisementDescription = (EditText)FindViewById(Resource.Id.editTextDescription);
			advertisementPrice = (EditText)FindViewById(Resource.Id.editTextPrice);
			mPhotoView1 = (ImageView)FindViewById(Resource.Id.photoView1);
			mButtonTakePicture1 = (Button)FindViewById(Resource.Id.buttonTakePicture1);
			mButtonTakePicture1.Tag = 1;
			mPhotoView2 = (ImageView)FindViewById(Resource.Id.photoView2);
			mButtonTakePicture2 = (Button)FindViewById(Resource.Id.buttonTakePicture2); mButtonTakePicture1.Tag = 1;
			mButtonTakePicture2.Tag = 2;
			mPhotoView3 = (ImageView)FindViewById(Resource.Id.photoView3);
			mButtonTakePicture3 = (Button)FindViewById(Resource.Id.buttonTakePicture3);
			mButtonTakePicture3.Tag = 3;
			buttonPublishAdvertisement = FindViewById<Button>(Resource.Id.buttonPublishAdvertisemenetItem);

			buttonPublishAdvertisement.Click += async (s, e) => await ButtonPublishAdvertisement_Click(s, e);
			mButtonTakePicture1.Click += MButtonTakePicture_Click;
			mButtonTakePicture2.Click += MButtonTakePicture_Click;
			mButtonTakePicture3.Click += MButtonTakePicture_Click;
		}


		private async Task ButtonPublishAdvertisement_Click(object sender, EventArgs e)
		{
			progress.ShowProgressDialog("Wysy�anie ogloszenia. Prosz� czeka�...");
			if (AdvertisementItemDataIsValidate())
			{
				var location = gpsLocationService.GetLocation();
				if (location.Longitude != 0.0 && location.Latitude != 0.0)
				{
					var photosBytesArraysList = GetPhotosByteArray(this.photosPaths);

					var photosListModel = await this.advertisementItemService.UploadNewAdvertisementPhotos(photosBytesArraysList);
					if (photosListModel != null)
					{
						var newAdvertisementModel = CreateNewAdvertisementItemModel(photosListModel);
						var success = await this.advertisementItemService.CreateNewAdvertisement(newAdvertisementModel);
						
						if (success)
						{
							foreach (var tempPath in tempPhotosPaths)
							{
								System.IO.File.Delete(tempPath);
							}
							AlertsService.ShowToast(this, "Pomy�lnie utworzone nowe og�oszenie");

							this.Finish();
						}
						else
						{
							AlertsService.ShowToast(this, "Nie uda�o si� utworzy� nowego og�oszenia. Spr�buj ponownie");
						}

					}
				}
				else
				{
					//lokalizacja jest chujowa
					Toast.MakeText(this, "Wsp�lrz�dne lokalizacji s� zerowe", ToastLength.Long).Show();
				}
			}
			progress.CloseProgressDialog();
		}

		private IEnumerable<byte[]> GetPhotosByteArray(List<string> photosPaths)
		{
			foreach (var photoPath in photosPaths)
			{
				var bytesArray = System.IO.File.ReadAllBytes(photoPath);
				var resized = this.bitmapOperationService.ResizeImageAndGetByteArray(bytesArray);

				yield return resized;
			}
		}

		private NewAdvertisementItem CreateNewAdvertisementItemModel(AdvertisementItemPhotosPaths photosListModel)
		{
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

		private bool AdvertisementItemDataIsValidate()
		{
			bool isValidate = true;
			if (advertisementTitle.Text.Length < 5)
			{
				isValidate = false;
				advertisementTitle.Error = "Tytu� musi zawiera� min. 15 znak�w";
				focusView = advertisementTitle;
			}
			else if (advertisementDescription.Text.Length < 5)
			{
				isValidate = false;
				advertisementDescription.Error = "Opis musi zawiera� min 20 znak�w";
				focusView = advertisementDescription;
			}
			else if (advertisementPrice.Text.Length == 0)
			{
				isValidate = false;
				advertisementPrice.Error = "Cena musi zosta� podana";
				focusView = advertisementPrice;
			}
			else if (this.photosPaths.Count == 0)
			{
				Toast.MakeText(this, "Nie dodano �adnego zdj�cia", ToastLength.Long).Show();
				isValidate = false;
			}
			return isValidate;
		}

		private void MButtonTakePicture_Click(object sender, EventArgs e)
		{
			var btnNr = (int)(sender as Button).Tag;
			TakePhoto(btnNr);
		}

		private void TakePhoto(int photoNr)
		{
			var takingPhotoKindNames = Enum.GetValues(typeof(GetPhotoKind)).GetAllItemsDisplayNames();
			AlertsService.ShowSingleSelectListString(this, takingPhotoKindNames.ToArray(), s =>
			{
				var selectedTakingPhotoKind = s.GetEnumValueByDisplayName<GetPhotoKind>();
				photoIsTaking = true;
				switch (selectedTakingPhotoKind)
				{
					case GetPhotoKind.TakeNewPhotoFromCamera:
						TakePhotoFromCamera(photoNr);
						break;
					case GetPhotoKind.TakeExistingPhotoFromStorage:
						TakePhotoFromStorage(photoNr);
						break;
					default:
						break;
				}
			});
		}

		private void TakePhotoFromStorage(int photoNr)
		{
			var selectExistingPhotoIntent = new Intent();
			selectExistingPhotoIntent.SetType("image/*");
			selectExistingPhotoIntent.SetAction(Intent.ActionGetContent);
			StartActivityForResult(Intent.CreateChooser(selectExistingPhotoIntent, "Wybierz zdj�cie"), photoNr);
		}

		private void TakePhotoFromCamera(int photoNr)
		{
			Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
			if (takePictureIntent.ResolveActivity(PackageManager) != null)
			{
				Java.IO.File photoFile = null;
				try
				{
					photoFile = CreateImageFile(photoNr);
				}
				catch (Java.IO.IOException ex)
				{
					Toast.MakeText(this, "Co� sie zjeba�o ze zdjeciem", ToastLength.Long).Show();
				}
				if (photoFile != null)
				{
					takePictureIntent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(photoFile));

					base.StartActivityForResult(takePictureIntent, photoNr);
				}
			}
		}

		private Java.IO.File CreateImageFile(int photoNr)
		{
			String timeStamp = DateTime.Now.ToString();
			String imageFileName = "JPEG_" + timeStamp + "_";
			Java.IO.File storageDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);

			Java.IO.File image = Java.IO.File.CreateTempFile(
					imageFileName,  /* prefix */
					".jpg",         /* suffix */
					storageDir      /* directory */
			);

			SavePhotoPath(photoNr, image.AbsolutePath);

			return image;
		}

		private void SavePhotoPath(int photoNr, string path)
		{
			var photoPathINdex = photoNr - 1;
			if (this.photosPaths.Count >= photoNr)
			{
				this.photosPaths[photoPathINdex] = path;
			}
			else
			{
				this.photosPaths.Add(path);
			}
		}
	}
}