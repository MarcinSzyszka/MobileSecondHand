using System;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.App.Notifications;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Consts;
using MobileSecondHand.Services.Keywords;
using MobileSecondHand.Services.Google_Api;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.App.Activities;
using Android.App;
using MobileSecondHand.Services.Categories;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.API.Models.Shared.Consts;
using Refractored.Controls;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Extensions;
using Android.Provider;
using Android.Content.PM;
using Android.Runtime;
using Android.Graphics;
using MobileSecondHand.Services.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using MobileSecondHand.App.Receivers;

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
			this.textViewUserName = activity.FindViewById<TextView>(Resource.Id.textViewUserName);
			this.textViewNotificationsState = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsState);
			this.textViewChatState = activity.FindViewById<TextView>(Resource.Id.textViewChatState);
			this.textViewNotificationsRadius = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsRadius);
			this.textViewKeywords = activity.FindViewById<TextView>(Resource.Id.textViewKeywords);
			this.textViewNotificationsSizes = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsSize);
			this.textViewHomeLocalization = activity.FindViewById<TextView>(Resource.Id.textViewHomeLocalization);
			this.imgAppInfo = activity.FindViewById<ImageButton>(Resource.Id.imgAppInfo);
			this.imgAppInfo.Click += (s, e) =>
			{
				var intent = new Intent(activity, typeof(AppInfoAndContactActivity));
				activity.StartActivity(intent);
			};

			//conversations
			SetupChatStateView(activity);

			//notifications
			SetupNotificationsStateView(activity);


			this.imgBtnKeywords = activity.FindViewById<ImageButton>(Resource.Id.imgBtnKeywords);
			this.imgBtnKeywords.Click += async (sender, args) =>
			{
				var userSelectesKeywordsNames = appSettings.Keywords.Select(k => k.Value).ToList();
				await this.categoriesHelper.ShowCategoriesListAndMakeAction(userSelectesKeywordsNames, MethodToExecuteAfterCategoriesSelect);
			};

			this.imgBtnSizes = activity.FindViewById<ImageButton>(Resource.Id.imgBtnNotificationsSize);
			this.imgBtnSizes.Click += (sender, args) =>
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
			};



			this.imgBtnHomeLocalization = activity.FindViewById<ImageButton>(Resource.Id.imgBtnHomeLocalization);
			this.imgBtnHomeLocalization.Click += (sender, args) =>
			{
				//gpsService
				var confirmMessage = "Czy na pewno chcesz ustaliæ aktualn¹ lokalizacjê i ustawiæ j¹ jako domow¹?";
				AlertsService.ShowConfirmDialog(activity, confirmMessage, async () =>
				{
					this.progressDialogHelper.ShowProgressDialog("Trwa ustalanie Twojej aktualnej lokalizacji");
					try
					{
						if (!this.gpsService.CanGetLocation)
						{
							AlertsService.ShowLongToast(activity, "W³¹cz gps w ustawieniach");
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
								var infoMessage = String.Format("Adres lokalizacji to w przybli¿eniu: {0}. Na odczyt lokalizacji wp³ywa wiele czynników dlatego wiemy, ¿e adres mo¿e nie byæ w 100% idealny. Twoja lokalizacja wraz z adresem nie bêdzie nikomu udostêpniona.", address);
								AlertsService.ShowAlertDialog(activity, infoMessage);
							}
						}

					}
					catch (Exception)
					{
						AlertsService.ShowLongToast(activity, "Wyst¹pi³ problem z okreœleniem Twojej lokalizacji. Spróbuj ponownie póŸniej");
					}
					finally
					{
						this.progressDialogHelper.CloseProgressDialog();
					}

				});
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
			//wysy³anko do serwera
			try
			{
				this.progressDialogHelper.ShowProgressDialog("Zapisywanie zdjêcia profilowego");
				var photoResized = await this.bitmapOperationService.GetScaledDownPhotoByteArray(profilePhotoPath, true);
				var profileImagePath = System.IO.Path.Combine(Application.Context.FilesDir.AbsolutePath, "profilePicture.jpg");
				System.IO.File.WriteAllBytes(profileImagePath, photoResized);
				appSettings.ProfileImagePath = profileImagePath;
				SetAppSettings(appSettings);
				await signInService.UploadUserProfilePhoto(activity.BearerToken, photoResized);
			}
			catch (Exception exc)
			{
				AlertsService.ShowLongToast(activity, "Wyst¹pi³ b³¹d.");
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
			AlertsService.ShowConfirmDialog(activity, "Czy chcesz zmieniæ zdjêcie profilowe?", changeProfilePiuctureAction);
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
			});
		}

		private void TakePhotoFromStorage()
		{
			profilePhotoPath = null;
			var selectExistingPhotoIntent = new Intent();
			selectExistingPhotoIntent.SetType("image/*");
			selectExistingPhotoIntent.SetAction(Intent.ActionGetContent);
			activity.StartActivityForResult(Intent.CreateChooser(selectExistingPhotoIntent, "Wybierz zdjêcie"), PHOTO_REQUEST_KEY);
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
					Toast.MakeText(activity, "Nie uda³o siê utworzyæ pliku dla zdjêcia.", ToastLength.Long).Show();
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
			String timeStamp = DateTime.Now.ToString();
			String imageFileName = "JPEG_" + timeStamp + "_";
			Java.IO.File storageDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);

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
			this.notificationsStateSwitch.Click += (sender, args) =>
			{
				var confirmMessage = String.Format("Czy na pewno chcesz {0} powiadomienia o nowoœciach?", !notificationsStateSwitch.Checked ? "wy³¹czyæ" : "w³¹czyæ");
				AlertsService.ShowConfirmDialog(activity, confirmMessage, () =>
				{
					if (!notificationsStateSwitch.Checked)
					{
						appSettings.NotificationsDisabled = true;
						//activity.StopService(new Intent(activity.ApplicationContext, typeof(NewsService)));
					}
					else
					{
						appSettings.NotificationsDisabled = false;
						//activity.StartService(new Intent(activity.ApplicationContext, typeof(NewsService)));
					}

					SetAppSettings(appSettings);
					this.textViewNotificationsState.Text = notificationsStateSwitch.Checked ? "w³¹czone" : "wy³¹czone";
				},
				() =>
				{
					this.notificationsStateSwitch.Checked = !notificationsStateSwitch.Checked;
				});
			};

			this.imgBtnRadius = activity.FindViewById<ImageButton>(Resource.Id.imgBtnRadius);
			imgBtnRadius.Click += (sender, args) =>
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
				});
			};
		}

		private void SetupChatStateView(BaseActivity activity)
		{
			this.chatStateSwitch = activity.FindViewById<SwitchCompat>(Resource.Id.switchChatState);
			this.chatStateSwitch.Click += (sender, args) =>
			{
				var confirmMessage = String.Format("Czy na pewno chcesz {0} czat?", !chatStateSwitch.Checked ? "wy³¹czyæ" : "w³¹czyæ");
				AlertsService.ShowConfirmDialog(activity, confirmMessage, () =>
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
						WakeUpAlarmReceiver.SetWakeUpAlarmRepeating(activity.ApplicationContext);
					}

					SetAppSettings(appSettings);
					this.textViewChatState.Text = chatStateSwitch.Checked ? "w³¹czony" : "wy³¹czony";
				},
				() =>
				{
					this.chatStateSwitch.Checked = !chatStateSwitch.Checked;
				});
			};
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
				result = "nieustawiona";
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

			this.textViewNotificationsRadius.Text = settingsModel.LocationSettings.MaxDistance < ValueConsts.MAX_DISTANCE_VALUE ? String.Format("{0} km", settingsModel.LocationSettings.MaxDistance.ToString()) : "bez ograniczeñ";

		}

		private void SetNotificationsSettings()
		{
			if (!appSettings.NotificationsDisabled)
			{
				this.textViewNotificationsState.Text = "w³¹czone";

				this.notificationsStateSwitch.Checked = true;
			}
			else
			{
				this.textViewNotificationsState.Text = "wy³¹czone";
				this.notificationsStateSwitch.Checked = false;
			}
		}

		private void SetChatSettings()
		{
			if (appSettings.ChatDisabled && !MessengerService.ServiceIsRunning)
			{
				this.textViewChatState.Text = "wy³¹czony";
				this.chatStateSwitch.Checked = false;
			}
			else
			{
				this.textViewChatState.Text = "w³¹czony";
				this.chatStateSwitch.Checked = true;
			}
		}
	}
}