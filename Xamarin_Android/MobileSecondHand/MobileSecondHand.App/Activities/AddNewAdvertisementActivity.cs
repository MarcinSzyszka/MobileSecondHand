using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Extensions;
using MobileSecondHand.API.Models.Shared.Categories;
using MobileSecondHand.API.Models.Shared.Advertisements;
using Newtonsoft.Json;
using MobileSecondHand.App.Consts;
using MobileSecondHand.Models.Advertisement;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Nowe og³oszenie", ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class AddNewAdvertisementActivity : BaseActivity
	{
		BitmapOperationService bitmapOperationService;
		GpsLocationService gpsLocationService;
		CategoriesSelectingHelper categoriesSelectingHelper;
		SizeSelectingHelper sizeSelectingHelper;
		private EditText advertisementDescription;
		private EditText advertisementPrice;
		private EditText advertisementTitle;
		private ProgressDialogHelper progress;
		private RadioButton rdBtnOnlyForSell;
		private List<string> photosPaths;
		private int REQUEST_TAKE_PHOTO_1 = 1;
		private int REQUEST_TAKE_PHOTO_2 = 2;
		private int REQUEST_TAKE_PHOTO_3 = 3;
		private View focusView;
		IAdvertisementItemService advertisementItemService;
		private ImageView mPhotoView1;
		private Button mButtonTakePicture1;
		private ImageView btnChoseCategory;
		private ImageView btnChoseSize;
		private TextView textViewChodesdCategory;
		private TextView textViewChodesdSize;

		private ImageView mPhotoView2;
		private Button mButtonTakePicture2;
		private ImageView mPhotoView3;
		private Button mButtonTakePicture3;
		private List<string> tempPhotosPaths;
		private TextView photoDivider2;
		private TextView photoDivider3;
		CategoryInfoModel categoryInfoModel;
		private ClothSize size;
		private Action CreateAdvertMenuItemClicked;
		bool isEditingMode;
		private AdvertisementEditModel editModel;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.bitmapOperationService = new BitmapOperationService();
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.gpsLocationService = new GpsLocationService(this, null);
			this.categoriesSelectingHelper = new CategoriesSelectingHelper(this, bearerToken);
			this.sizeSelectingHelper = new SizeSelectingHelper(this);
			this.categoryInfoModel = new CategoryInfoModel();
			SetContentView(Resource.Layout.AddNewAdvertisementActivity);
			base.SetupToolbar();
			SetupViews();
			photosPaths = new List<string>();
			tempPhotosPaths = new List<string>();
			await GetExtrasIsExistAndFillForm();
		}

		private async Task GetExtrasIsExistAndFillForm()
		{
			var editMOdelString = Intent.GetStringExtra(ExtrasKeys.ADVERTISEMENT_ITEM_EDIT_MODEL);
			if (String.IsNullOrEmpty(editMOdelString))
			{
				//not edit
				return;
			}
			Title = "Edycja og³oszenia";
			this.editModel = JsonConvert.DeserializeObject<AdvertisementEditModel>(editMOdelString);
			categoryInfoModel = editModel.CategoryInfoModel;
			textViewChodesdCategory.Text = categoryInfoModel.Name;
			advertisementTitle.Text = editModel.Title;
			advertisementDescription.Text = editModel.Description;
			advertisementPrice.Text = editModel.Price.ToString();
			size = editModel.Size;
			textViewChodesdSize.Text = editModel.Size.GetDisplayName();
			rdBtnOnlyForSell.Selected = editModel.IsOnlyForSell;
			photosPaths = editModel.PhotosPaths;
			for (int i = 0; i < photosPaths.Count; i++)
			{
				var photoNr = i + 1;
				await SetPhoto(photoNr);
			}

		}

		protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (resultCode == Result.Ok && (requestCode == REQUEST_TAKE_PHOTO_1 || requestCode == REQUEST_TAKE_PHOTO_2 || requestCode == REQUEST_TAKE_PHOTO_3))
			{
				if (this.photosPaths.Count >= requestCode && this.photosPaths[requestCode - 1] != null)
				{
					await SetPhoto(requestCode);
				}
				if (data != null)
				{
					var file = CreateImageFile(requestCode);
					tempPhotosPaths.Add(this.bitmapOperationService.SavePhotoFromUriAndReturnPhysicalPath(data.Data, file, this));
					await SetPhoto(requestCode);
				}
			}
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.mainActivityMenu, menu);
			if (menu != null)
			{
				menu.FindItem(Resource.Id.showFavouritesList).SetVisible(false);
				menu.FindItem(Resource.Id.applyFilterOptions).SetVisible(true);
				menu.FindItem(Resource.Id.clearFilterOptions).SetVisible(false);
				menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(false);
				menu.FindItem(Resource.Id.chat).SetVisible(false);
				menu.FindItem(Resource.Id.choosingAdvertisementsList).SetVisible(false);
				menu.FindItem(Resource.Id.home).SetVisible(false);
			}

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var handled = false;
			switch (item.ItemId)
			{
				case Resource.Id.applyFilterOptions:
					CreateAdvertMenuItemClicked();
					handled = true;
					break;
			}

			return handled;
		}

		private async Task SetPhoto(int photoNr)
		{
			Bitmap resizedImage = await this.bitmapOperationService.GetScaledDownBitmapForDisplayAsync(this.photosPaths[photoNr - 1]);
			switch (photoNr)
			{
				case 1:
					{
						mPhotoView1.SetImageBitmap(resizedImage);
						mButtonTakePicture1.Text = "Zmieñ zdjêcie";
						photoDivider2.Visibility = ViewStates.Visible;
						mPhotoView2.Visibility = ViewStates.Visible;
						mButtonTakePicture2.Visibility = ViewStates.Visible;
						break;
					}
				case 2:
					{
						mPhotoView2.SetImageBitmap(resizedImage);
						mButtonTakePicture2.Text = "Zmieñ zdjêcie";
						photoDivider3.Visibility = ViewStates.Visible;
						mPhotoView3.Visibility = ViewStates.Visible;
						mButtonTakePicture3.Visibility = ViewStates.Visible;
						break;
					}
				case 3:
					{
						mPhotoView3.SetImageBitmap(resizedImage);
						mButtonTakePicture3.Text = "Zmieñ zdjêcie";
						break;
					}
				default:
					break;
			}
		}

		private void SetupViews()
		{
			this.CreateAdvertMenuItemClicked = async () =>
			{
				progress.ShowProgressDialog("Wysy³anie ogloszenia. Proszê czekaæ...");
				try
				{
					await CreateAdvertisement();
				}
				catch { AlertsService.ShowShortToast(this, "Wyst¹pi³ nieoczekiwany b³¹d..."); }
				finally
				{
					progress.CloseProgressDialog();
				}

			};
			rdBtnOnlyForSell = (RadioButton)FindViewById(Resource.Id.rdBtnOnlyForSell);
			progress = new ProgressDialogHelper(this);
			advertisementTitle = (EditText)FindViewById(Resource.Id.editTextTitle);
			advertisementDescription = (EditText)FindViewById(Resource.Id.editTextDescription);
			advertisementPrice = (EditText)FindViewById(Resource.Id.editTextPrice);
			mPhotoView1 = (ImageView)FindViewById(Resource.Id.photoView1);
			mButtonTakePicture1 = (Button)FindViewById(Resource.Id.buttonTakePicture1);
			mButtonTakePicture1.Tag = 1;
			photoDivider2 = (TextView)FindViewById(Resource.Id.photoView2Divider);
			mPhotoView2 = (ImageView)FindViewById(Resource.Id.photoView2);
			mButtonTakePicture2 = (Button)FindViewById(Resource.Id.buttonTakePicture2); mButtonTakePicture1.Tag = 1;
			mButtonTakePicture2.Tag = 2;
			photoDivider3 = (TextView)FindViewById(Resource.Id.photoView3Divider);
			mPhotoView3 = (ImageView)FindViewById(Resource.Id.photoView3);
			mButtonTakePicture3 = (Button)FindViewById(Resource.Id.buttonTakePicture3);
			mButtonTakePicture3.Tag = 3;

			btnChoseCategory = (ImageView)FindViewById(Resource.Id.btnAddAdvCategoryChosing);
			btnChoseCategory.Click += async (s, e) => await BtnChoseCategory_Click(s, e);
			textViewChodesdCategory = (TextView)FindViewById(Resource.Id.textViewChosedCategory);
			textViewChodesdCategory.Click += async (s, e) => await BtnChoseCategory_Click(s, e);

			btnChoseSize = (ImageView)FindViewById(Resource.Id.btnAddSize);
			btnChoseSize.Click += BtnChoseSize_Click;
			textViewChodesdSize = (TextView)FindViewById(Resource.Id.textViewSelectedSize);
			textViewChodesdSize.Click += BtnChoseSize_Click;

			mButtonTakePicture1.Click += MButtonTakePicture_Click;
			mButtonTakePicture2.Click += MButtonTakePicture_Click;
			mButtonTakePicture3.Click += MButtonTakePicture_Click;
		}

		private void BtnChoseSize_Click(object sender, EventArgs e)
		{
			var selectedName = this.size != default(ClothSize) ? this.size.GetDisplayName() : null;
			Action<ClothSize> methodAfterSelect = (s) =>
			{
				this.size = s;
				this.textViewChodesdSize.Text = size.GetDisplayName();
			};
			this.sizeSelectingHelper.ShowSizeSingleSelectAndMakeAction(methodAfterSelect, selectedName);

		}

		private async Task BtnChoseCategory_Click(object sender, EventArgs e)
		{
			try
			{
				await this.categoriesSelectingHelper.ShowCategoriesSingleSelectAndMakeAction(GetMethodToExecuteAfterCategoryChosing(), this.categoryInfoModel.Name);
			}
			catch (Exception exc)
			{
				AlertsService.ShowShortToast(this, "Wsytapil wyjatek: " + exc);
				//dziwne wyjatki wystepuj¹ gdy nie zlapie tego w try catch
			}

		}

		private Action<int, string> GetMethodToExecuteAfterCategoryChosing()
		{
			return (id, name) =>
			{
				this.textViewChodesdCategory.Text = name;
				this.categoryInfoModel.Id = id;
				this.categoryInfoModel.Name = name;
			};
		}

		private async Task CreateAdvertisement()
		{
			if (AdvertisementItemDataIsValidate())
			{
				var location = gpsLocationService.GetLocation();
				if (location.Longitude != 0.0 && location.Latitude != 0.0)
				{
					var photosBytesArraysList = await GetPhotosByteArray(this.photosPaths);

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
							var message = editModel == null ? "Pomyœlnie utworzone nowe og³oszenie" : "Pomyœlnie zaktualizowano og³oszenie";
							AlertsService.ShowLongToast(this, message);

							this.Finish();
						}
						else
						{
							AlertsService.ShowLongToast(this, "Nie uda³o siê utworzyæ nowego og³oszenia. Spróbuj ponownie");
						}
					}
				}
				else
				{
					Toast.MakeText(this, "Nie mogê ustaliæ wspó³rzêdnych. Upewnij siê, ¿e masz w³¹czon¹ lokalizacje.", ToastLength.Long).Show();
				}
			}
		}

		private async Task<IEnumerable<byte[]>> GetPhotosByteArray(List<string> photosPaths)
		{
			var byteArrayList = new List<byte[]>();
			foreach (var photoPath in photosPaths)
			{
				var byteArrayPhoto = await this.bitmapOperationService.GetScaledDownPhotoByteArray(photoPath);
				byteArrayList.Add(byteArrayPhoto);
			}

			return byteArrayList;
		}

		private NewAdvertisementItem CreateNewAdvertisementItemModel(AdvertisementItemPhotosNames photosListModel)
		{
			var location = this.gpsLocationService.GetLocation();
			NewAdvertisementItem model = new NewAdvertisementItem();
			model.Id = editModel != null ? editModel.Id : -1;
			model.AdvertisementTitle = advertisementTitle.Text;
			model.AdvertisementDescription = advertisementDescription.Text;
			model.Size = this.size;
			model.Latitude = location.Latitude;
			model.Longitude = location.Longitude;
			model.IsOnlyForSell = rdBtnOnlyForSell.Checked;
			model.AdvertisementPrice = Int32.Parse(advertisementPrice.Text);
			model.Category = this.categoryInfoModel;
			model.PhotosNames = photosListModel.PhotosNames;

			return model;
		}

		private bool AdvertisementItemDataIsValidate()
		{
			bool isValidate = true;
			if (advertisementTitle.Text.Length < 5)
			{
				isValidate = false;
				advertisementTitle.Error = "Tytu³ musi zawieraæ min. 5 znaków";
				focusView = advertisementTitle;
			}
			else if (advertisementDescription.Text.Length < 10)
			{
				isValidate = false;
				advertisementDescription.Error = "Opis musi zawieraæ min 10 znaków";
				focusView = advertisementDescription;
			}
			else if (advertisementPrice.Text.Length == 0)
			{
				isValidate = false;
				advertisementPrice.Error = "Cena musi zostaæ podana";
				focusView = advertisementPrice;
			}
			else if (this.photosPaths.Count == 0)
			{
				Toast.MakeText(this, "Nie dodano ¿adnego zdjêcia", ToastLength.Long).Show();
				isValidate = false;
			}
			else if (this.categoryInfoModel.Id == 0)
			{
				Toast.MakeText(this, "Nie wybrano kategorii", ToastLength.Long).Show();
				isValidate = false;
			}
			else if (this.size == default(ClothSize))
			{
				Toast.MakeText(this, "Nie wybrano rozmiaru", ToastLength.Long).Show();
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
			}, dialogTitle: "Wybierz Ÿród³o");
		}

		private void TakePhotoFromStorage(int photoNr)
		{
			SavePhotoPath(photoNr, null);
			var selectExistingPhotoIntent = new Intent();
			selectExistingPhotoIntent.SetType("image/*");
			selectExistingPhotoIntent.SetAction(Intent.ActionGetContent);
			StartActivityForResult(Intent.CreateChooser(selectExistingPhotoIntent, "Wybierz zdjêcie"), photoNr);

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
					Toast.MakeText(this, "Nie uda³o siê utworzyæ pliku dla zdjêcia.", ToastLength.Long).Show();
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