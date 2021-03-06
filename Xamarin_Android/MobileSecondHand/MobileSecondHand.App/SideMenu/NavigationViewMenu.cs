using System;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Widget;
using Android.Support.V7.Widget;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Activities;
using Android.App;
using MobileSecondHand.API.Models.Shared.Consts;
using Refractored.Controls;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Extensions;
using Android.Provider;
using MobileSecondHand.Services.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using MobileSecondHand.API.Services.Shared.GoogleApi;

namespace MobileSecondHand.App.SideMenu
{
	public class NavigationViewMenu
	{
		ISignInService signInService;
		CategoriesSelectingHelper categoriesHelper;
		private SharedPreferencesHelper sharedPreferencesHelper;
		BitmapOperationService bitmapOperationService;
		private AppSettingsModel appSettings;
		ProgressDialogHelper progressDialogHelper;
		GpsLocationService gpsService;
		IGoogleMapsAPIService googleMapsAPIService;
		private SwitchCompat chatStateSwitch;
		private ImageButton imgBtnHomeLocalization;
		private ImageButton imgBtnKeywords;
		private SwitchCompat notificationsStateSwitch;
		private ImageButton imgBtnRadius;
		private TextView textViewChatState;
		private TextView textViewHomeLocalization;
		private TextView textViewKeywords;
		private TextView textViewNotificationsRadius;
		private TextView textViewNotificationsState;
		private TextView textViewUserName;
		private TextView textViewNotificationsSizes;
		private ImageButton imgBtnSizes;
		private BaseActivityWithNavigationDrawer activity;
		CircleImageView userProfilePhoto;
		public const int PHOTO_REQUEST_KEY = 111;
		private string profilePhotoPath;
		private string profileTempPhotoPath;
		private SizeSelectingHelper sizeSelectingHelper;
		private ImageButton imgAppInfo;

		public NavigationViewMenu(BaseActivityWithNavigationDrawer activity, SharedPreferencesHelper sharedPreferencesHelper)
		{
			this.activity = activity;
			signInService = new SignInService();
			this.bitmapOperationService = new BitmapOperationService();
			this.progressDialogHelper = new ProgressDialogHelper(activity);
			this.sharedPreferencesHelper = sharedPreferencesHelper;
			this.gpsService = GpsLocationService.GetServiceInstance(activity);
			this.googleMapsAPIService = new GoogleMapsAPIService();
			this.categoriesHelper = new CategoriesSelectingHelper(activity, (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN));
			this.sizeSelectingHelper = new SizeSelectingHelper(activity);
			this.appSettings = SharedPreferencesHelper.GetAppSettings(activity);
			SetupViews(activity);
		}

		private void SetupViews(BaseActivity activity)
		{
			this.userProfilePhoto = activity.FindViewById<CircleImageView>(Resource.Id.profile_image);
			DisplayProfilePhoto();
			userProfilePhoto.Click += UserProfilePhoto_Click;
			var imgBtnProfileImage = activity.FindViewById<ImageButton>(Resource.Id.imgBtnProfileImage);
			imgBtnProfileImage.Click += UserProfilePhoto_Click;
			this.textViewUserName = activity.FindViewById<TextView>(Resource.Id.textViewUserName);
			this.textViewNotificationsState = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsState);
			this.textViewChatState = activity.FindViewById<TextView>(Resource.Id.textViewChatState);
			this.textViewNotificationsRadius = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsRadius);
			this.textViewKeywords = activity.FindViewById<TextView>(Resource.Id.textViewKeywords);
			this.textViewNotificationsSizes = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsSize);
			this.textViewHomeLocalization = activity.FindViewById<TextView>(Resource.Id.textViewHomeLocalization);
			SetupChatStateView(activity);
			SetupNotificationsStateView(activity);
			SetNotificationsCategoriesViews(activity);
			SetNotificationsSizesViews(activity);
			SetHomeLocationViews(activity);
			SetAppInfoViews(activity);
		}

		private void SetHomeLocationViews(BaseActivity activity)
		{
			this.imgBtnHomeLocalization = activity.FindViewById<ImageButton>(Resource.Id.imgBtnHomeLocalization);
			this.imgBtnHomeLocalization.Click += ImgBtnHomeLocalization_Click;
			var homeLocLayout = activity.FindViewById<RelativeLayout>(Resource.Id.relLayoutHomeLocation);
			homeLocLayout.Click += ImgBtnHomeLocalization_Click;
		}

		private void ImgBtnHomeLocalization_Click(object sender, EventArgs e)
		{
			var confirmMessage = "Czy na pewno chcesz ustali� aktualn� lokalizacj� i ustawi� j� jako domow�?";
			AlertsService.ShowConfirmDialog(activity, confirmMessage, async () =>
			{
				this.progressDialogHelper.ShowProgressDialog("Trwa ustalanie Twojej aktualnej lokalizacji");
				try
				{
					if (!this.gpsService.CanGetLocation)
					{
						AlertsService.ShowLongToast(activity, "W��cz gps w ustawieniach");
					}
					else
					{
						var location = this.gpsService.GetLocation();
						var address = await this.googleMapsAPIService.GetAddress(location.Latitude, location.Longitude);
						appSettings.LocationSettings.Latitude = location.Latitude;
						appSettings.LocationSettings.Longitude = location.Longitude;
						appSettings.LocationSettings.LocationAddress = address;
						SetAppSettings(appSettings);
						SetHomeLocationSettings(appSettings);
						if (address != string.Empty)
						{
							var infoMessage = String.Format("Adres lokalizacji to w przybli�eniu: {0}. Na odczyt lokalizacji wp�ywa wiele czynnik�w dlatego wiemy, �e adres mo�e nie by� w 100% idealny. Twoja lokalizacja wraz z adresem nie b�dzie nikomu udost�pniona.", address);
							AlertsService.ShowAlertDialog(activity, infoMessage);
						}
					}

				}
				catch (Exception)
				{
					AlertsService.ShowLongToast(activity, "Wyst�pi� problem z okre�leniem Twojej lokalizacji. Spr�buj ponownie p�niej");
				}
				finally
				{
					this.progressDialogHelper.CloseProgressDialog();
				}

			});
		}

		private void SetNotificationsSizesViews(BaseActivity activity)
		{
			this.imgBtnSizes = activity.FindViewById<ImageButton>(Resource.Id.imgBtnNotificationsSize);
			this.imgBtnSizes.Click += ImgBtnSizes_Click;
			var newsSizesLayout = activity.FindViewById<RelativeLayout>(Resource.Id.relLayoutNewsSizes);
			newsSizesLayout.Click += ImgBtnSizes_Click;
		}

		private void ImgBtnSizes_Click(object sender, EventArgs e)
		{
			var selectedSizesNames = new List<String>();
			foreach (var size in appSettings.Sizes)
			{
				selectedSizesNames.Add(size.GetDisplayName());
			}
			Action<List<ClothSize>> actionAfterSelect = (selectedSizes) =>
			{
				this.appSettings.Sizes = selectedSizes;
				SetAppSettings(appSettings);
				SetSizesSettings(appSettings);
			};

			this.sizeSelectingHelper.ShowSizesListAndMakeAction(selectedSizesNames, actionAfterSelect);
		}

		private void SetNotificationsCategoriesViews(BaseActivity activity)
		{
			this.imgBtnKeywords = activity.FindViewById<ImageButton>(Resource.Id.imgBtnKeywords);
			this.imgBtnKeywords.Click += async (sender, args) =>
			{
				var userSelectesKeywordsNames = appSettings.Keywords.Select(k => k.Value).ToList();
				await this.categoriesHelper.ShowCategoriesListAndMakeAction(userSelectesKeywordsNames, MethodToExecuteAfterCategoriesSelect);
			};
			var newsCatLayout = activity.FindViewById<RelativeLayout>(Resource.Id.relLayoutNewsCategories);
			newsCatLayout.Click += async (sender, args) =>
			{
				var userSelectesKeywordsNames = appSettings.Keywords.Select(k => k.Value).ToList();
				await this.categoriesHelper.ShowCategoriesListAndMakeAction(userSelectesKeywordsNames, MethodToExecuteAfterCategoriesSelect);
			};
		}

		private void SetAppInfoViews(BaseActivity activity)
		{
			this.imgAppInfo = activity.FindViewById<ImageButton>(Resource.Id.imgAppInfo);
			this.imgAppInfo.Click += (s, e) =>
			{
				var intent = new Intent(activity, typeof(AppInfoAndContactActivity));
				activity.StartActivity(intent);
			};
			var appInfoLayout = activity.FindViewById<RelativeLayout>(Resource.Id.relLayoutAppInfos);
			appInfoLayout.Click += (s, e) =>
			{
				var intent = new Intent(activity, typeof(AppInfoAndContactActivity));
				activity.StartActivity(intent);
			};
		}

		private void SetSizesSettings(AppSettingsModel appSettings)
		{
			if (appSettings.Sizes.Count > 0)
			{
				var sb = new StringBuilder("");
				foreach (var size in appSettings.Sizes)
				{
					sb.Append(size.GetDisplayName());
					sb.Append("\r\n");
				}
				this.textViewNotificationsSizes.Text = sb.ToString();
			}
			else
			{
				this.textViewNotificationsSizes.Text = "Wszystkie";
			}
		}

		internal async Task OnAddPhotoTequestResult(Intent data)
		{

			if (profilePhotoPath != null)
			{
				await SetPhoto(profilePhotoPath);
			}
			else if (data != null)
			{
				var file = CreateImageFile();
				profileTempPhotoPath = this.bitmapOperationService.SavePhotoFromUriAndReturnPhysicalPath(data.Data, file, activity);
				await SetPhoto(profileTempPhotoPath);
				System.IO.File.Delete(profileTempPhotoPath);
			}

		}

		private async Task SetPhoto(string photoPath)
		{
			await SaveProfilePhoto(photoPath);
			DisplayProfilePhoto();
		}

		private async void DisplayProfilePhoto()
		{
			if (!String.IsNullOrEmpty(appSettings.ProfileImagePath))
			{
				this.userProfilePhoto.SetImageBitmap(await this.bitmapOperationService.GetScaledDownBitmapForDisplayAsync(appSettings.ProfileImagePath));
			}
		}

		private async Task SaveProfilePhoto(string profilePhotoPath)
		{
			//wysy�anko do serwera
			try
			{
				this.progressDialogHelper.ShowProgressDialog("Zapisywanie zdj�cia profilowego");
				var photoResized = await this.bitmapOperationService.GetScaledDownPhotoByteArray(profilePhotoPath, true, true);
				var profileImagePath = System.IO.Path.Combine(Application.Context.FilesDir.AbsolutePath, "profilePicture.jpg");
				System.IO.File.WriteAllBytes(profileImagePath, photoResized);
				appSettings.ProfileImagePath = profileImagePath;
				SetAppSettings(appSettings);
				await signInService.UploadUserProfilePhoto(activity.BearerToken, photoResized);
			}
			catch (Exception exc)
			{
				AlertsService.ShowLongToast(activity, "Wyst�pi� b��d.");
			}
			finally
			{
				this.progressDialogHelper.CloseProgressDialog();
			}

		}

		private void UserProfilePhoto_Click(object sender, EventArgs e)
		{
			Action changeProfilePiuctureAction = () =>
			{
				TakePhoto();
			};
			AlertsService.ShowConfirmDialog(activity, "Czy chcesz zmieni� zdj�cie profilowe?", changeProfilePiuctureAction);
		}

		private void TakePhoto()
		{
			var takingPhotoKindNames = Enum.GetValues(typeof(GetPhotoKind)).GetAllItemsDisplayNames();
			AlertsService.ShowSingleSelectListString(activity, takingPhotoKindNames.ToArray(), s =>
			{
				var selectedTakingPhotoKind = s.GetEnumValueByDisplayName<GetPhotoKind>();
				switch (selectedTakingPhotoKind)
				{
					case GetPhotoKind.TakeNewPhotoFromCamera:
						TakePhotoFromCamera();
						break;
					case GetPhotoKind.TakeExistingPhotoFromStorage:
						TakePhotoFromStorage();
						break;
					default:
						break;
				}
			},
			dialogTitle: "Wybierz �r�d�o");
		}

		private void TakePhotoFromStorage()
		{
			profilePhotoPath = null;
			var selectExistingPhotoIntent = new Intent();
			selectExistingPhotoIntent.SetType("image/*");
			selectExistingPhotoIntent.SetAction(Intent.ActionGetContent);
			activity.StartActivityForResult(Intent.CreateChooser(selectExistingPhotoIntent, "Wybierz zdj�cie"), PHOTO_REQUEST_KEY);
		}

		private void TakePhotoFromCamera()
		{
			Intent takePictureIntent = new Intent(MediaStore.ActionImageCapture);
			if (takePictureIntent.ResolveActivity(activity.PackageManager) != null)
			{
				Java.IO.File photoFile = null;
				try
				{
					photoFile = CreateImageFile();
				}
				catch (Java.IO.IOException ex)
				{
					Toast.MakeText(activity, "Nie uda�o si� utworzy� pliku dla zdj�cia.", ToastLength.Long).Show();
				}
				if (photoFile != null)
				{
					takePictureIntent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(photoFile));
					activity.StartActivityForResult(takePictureIntent, PHOTO_REQUEST_KEY);
				}
			}
		}

		private Java.IO.File CreateImageFile()
		{
			String timeStamp = DateTime.Now.ToString().Replace(" ", String.Empty).Replace("-", String.Empty).Replace(":", String.Empty);
			String imageFileName = "JPEG" + timeStamp;
			Java.IO.File storageDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);

			Java.IO.File image = Java.IO.File.CreateTempFile(
					imageFileName,  /* prefix */
					".jpg",         /* suffix */
					storageDir      /* directory */
			);

			profilePhotoPath = image.AbsolutePath;
			return image;
		}

		private Action<System.Collections.Generic.List<string>> MethodToExecuteAfterCategoriesSelect(System.Collections.Generic.IDictionary<int, string> allKeywords)
		{
			return selectedItemsNames =>
			{
				appSettings.Keywords.Clear();
				if (selectedItemsNames.Count != allKeywords.Count)
				{
					foreach (var itemName in selectedItemsNames)
					{
						appSettings.Keywords.Add(allKeywords.First(k => k.Value == itemName));
					}
				}

				SetAppSettings(appSettings);
				SetKeywordsSettings(appSettings);
			};
		}

		private void SetupNotificationsStateView(BaseActivity activity)
		{
			this.notificationsStateSwitch = activity.FindViewById<SwitchCompat>(Resource.Id.switchNotificationsState);
			this.notificationsStateSwitch.Click += NotificationsStateSwitch_Click;
			var newsNotificationsLayout = activity.FindViewById<RelativeLayout>(Resource.Id.relLayoutNewsNotifications);
			newsNotificationsLayout.Click += NewsNotificationsLayout_Click;

			this.imgBtnRadius = activity.FindViewById<ImageButton>(Resource.Id.imgBtnRadius);
			imgBtnRadius.Click += ImgBtnRadius_Click;
			var newsRadiusLayout = activity.FindViewById<RelativeLayout>(Resource.Id.relLayoutNewsRadius);
			newsRadiusLayout.Click += ImgBtnRadius_Click;

		}

		private void ImgBtnRadius_Click(object sender, EventArgs e)
		{
			string[] itemList = activity.Resources.GetStringArray(Resource.Array.notifications_radius);
			AlertsService.ShowSingleSelectListString(activity, itemList, selectedText =>
			{
				var resultRadius = 0;
				var selectedRadius = selectedText.Split(new char[] { ' ' })[0];
				int.TryParse(selectedRadius, out resultRadius);
				if (resultRadius == 0)
				{
					resultRadius = ValueConsts.MAX_DISTANCE_VALUE;
				}
				appSettings.LocationSettings.MaxDistance = resultRadius;
				SetAppSettings(appSettings);

				this.textViewNotificationsRadius.Text = selectedText;
			}, dialogTitle: "Wybierz dystans");
		}

		private void NewsNotificationsLayout_Click(object sender, EventArgs e)
		{
			Action actionAfterSwitch = () =>
			{
				notificationsStateSwitch.Checked = !notificationsStateSwitch.Checked;
				if (!notificationsStateSwitch.Checked)
				{
					appSettings.NotificationsDisabled = true;
				}
				else
				{
					appSettings.NotificationsDisabled = false;
				}

				SetAppSettings(appSettings);
				this.textViewNotificationsState.Text = notificationsStateSwitch.Checked ? "W��czone" : "Wy��czone";
			};

			if (!notificationsStateSwitch.Checked)
			{
				actionAfterSwitch();
			}
			else
			{
				var confirmMessage = String.Format("Czy na pewno chcesz wy��czy� powiadomienia o nowo�ciach?");
				AlertsService.ShowConfirmDialog(activity, confirmMessage, actionAfterSwitch);
			}

		}


		private void NotificationsStateSwitch_Click(object sender, EventArgs e)
		{
			Action actionAfterSwitch = () =>
			{
				if (!notificationsStateSwitch.Checked)
				{
					appSettings.NotificationsDisabled = true;
				}
				else
				{
					appSettings.NotificationsDisabled = false;
				}

				SetAppSettings(appSettings);
				this.textViewNotificationsState.Text = notificationsStateSwitch.Checked ? "W��czone" : "Wy��czone";
			};


			if (notificationsStateSwitch.Checked)
			{
				actionAfterSwitch();
			}
			else
			{
				var confirmMessage = String.Format("Czy na pewno chcesz wy��czy� powiadomienia o nowo�ciach?");
				AlertsService.ShowConfirmDialog(activity, confirmMessage, actionAfterSwitch, () =>
				{
					this.notificationsStateSwitch.Checked = !notificationsStateSwitch.Checked;
				});
			}
		}

		private void SetupChatStateView(BaseActivity activity)
		{
			this.chatStateSwitch = activity.FindViewById<SwitchCompat>(Resource.Id.switchChatState);
			this.chatStateSwitch.Click += ChatStateSwitch_Click;
			var chatStateLayout = activity.FindViewById<RelativeLayout>(Resource.Id.relLayoutChat);
			chatStateLayout.Click += ChatStateLayout_Click;

		}

		private void ChatStateLayout_Click(object sender, EventArgs e)
		{
			Action actionAfterSwitch = () =>
			{
				chatStateSwitch.Checked = !chatStateSwitch.Checked;
				if (!chatStateSwitch.Checked)
				{
					appSettings.ChatDisabled = true;
					activity.StopService(new Intent(activity.ApplicationContext, typeof(MessengerService)));
				}
				else
				{
					appSettings.ChatDisabled = false;
					activity.StartService(new Intent(activity.ApplicationContext, typeof(MessengerService)));
				}

				SetAppSettings(appSettings);
				this.textViewChatState.Text = chatStateSwitch.Checked ? "W��czony" : "Wy��czony";
			};
			if (!chatStateSwitch.Checked)
			{
				actionAfterSwitch();
			}
			else
			{
				var confirmMessage = "Czy na pewno chcesz wy��czy� czat?";
				AlertsService.ShowConfirmDialog(activity, confirmMessage, actionAfterSwitch);

			}
		}

		private void ChatStateSwitch_Click(object sender, EventArgs e)
		{
			Action actionAfterSwitch = () =>
			{
				if (!chatStateSwitch.Checked)
				{
					appSettings.ChatDisabled = true;
					activity.StopService(new Intent(activity.ApplicationContext, typeof(MessengerService)));
				}
				else
				{
					appSettings.ChatDisabled = false;
					activity.StartService(new Intent(activity.ApplicationContext, typeof(MessengerService)));
				}

				SetAppSettings(appSettings);
				this.textViewChatState.Text = chatStateSwitch.Checked ? "W��czony" : "Wy��czony";
			};
			if (chatStateSwitch.Checked)
			{
				actionAfterSwitch();
			}
			else
			{
				var confirmMessage = "Czy na pewno chcesz wy��czy� czat?";
				AlertsService.ShowConfirmDialog(activity, confirmMessage, actionAfterSwitch,
				() =>
				{
					this.chatStateSwitch.Checked = !chatStateSwitch.Checked;
				});
			}

		}

		internal void SetupMenu()
		{
			textViewUserName.Text = appSettings.UserName;
			SetChatSettings();
			SetNotificationsSettings();
			SetKeywordsSettings(appSettings);
			SetSizesSettings(appSettings);
			SetHomeLocationSettings(appSettings);
		}

		private void SetAppSettings(AppSettingsModel appSettings)
		{
			this.sharedPreferencesHelper.SetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS, appSettings);
		}

		private void SetHomeLocationSettings(AppSettingsModel settingsModel)
		{
			var result = String.Empty;

			if (settingsModel.LocationSettings.Latitude > 0)
			{
				if (settingsModel.LocationSettings.LocationAddress != String.Empty)
				{
					result = settingsModel.LocationSettings.LocationAddress;
				}
				else
				{
					result = String.Format("Lat {0}, Lon {1}", settingsModel.LocationSettings.Latitude, settingsModel.LocationSettings.Longitude);
				}
			}
			else
			{
				result = "Nieustawiona";
			}

			this.textViewHomeLocalization.Text = result;

		}

		private void SetKeywordsSettings(AppSettingsModel settingsModel)
		{
			if (settingsModel.Keywords.Count > 0)
			{
				var sb = new StringBuilder("");
				foreach (var keyword in settingsModel.Keywords)
				{
					sb.Append(keyword.Value);
					sb.Append("\r\n");
				}
				this.textViewKeywords.Text = sb.ToString();
			}
			else
			{
				this.textViewKeywords.Text = "Wszystkie";
			}

			this.textViewNotificationsRadius.Text = settingsModel.LocationSettings.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE ? String.Format("{0} km", settingsModel.LocationSettings.MaxDistance.ToString()) : "Bez ogranicze�";

		}

		private void SetNotificationsSettings()
		{
			if (!appSettings.NotificationsDisabled)
			{
				this.textViewNotificationsState.Text = "W��czone";

				this.notificationsStateSwitch.Checked = true;
			}
			else
			{
				this.textViewNotificationsState.Text = "Wy��czone";
				this.notificationsStateSwitch.Checked = false;
			}
		}

		private void SetChatSettings()
		{
			if (appSettings.ChatDisabled && !MessengerService.ServiceIsRunning)
			{
				this.textViewChatState.Text = "Wy��czony";
				this.chatStateSwitch.Checked = false;
			}
			else
			{
				this.textViewChatState.Text = "W��czony";
				this.chatStateSwitch.Checked = true;
			}
		}
	}
}