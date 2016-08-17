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

namespace MobileSecondHand.App.SideMenu
{
	public class NavigationViewMenu
	{
		IKeyworsService keyworsService;
		private SharedPreferencesHelper sharedPreferencesHelper;
		private AppSettingsModel appSettings;
		ProgressDialogHelper progressDialogHelper;
		GpsLocationService gpsService;
		IGoogleMapsAPIService googleMapsAPIService;
		private SwitchCompat chatStateSwitch;
		private ImageButton imgBtnConversations;
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

		public NavigationViewMenu(BaseActivity activity, SharedPreferencesHelper sharedPreferencesHelper)
		{
			this.progressDialogHelper = new ProgressDialogHelper(activity);
			this.keyworsService = new KeywordsService();
			this.sharedPreferencesHelper = sharedPreferencesHelper;
			this.gpsService = GpsLocationService.GetServiceInstance(activity);
			this.googleMapsAPIService = new GoogleMapsAPIService();
			SetupViews(activity);
		}

		private void SetupViews(BaseActivity activity)
		{
			this.textViewUserName = activity.FindViewById<TextView>(Resource.Id.textViewUserName);
			this.textViewNotificationsState = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsState);
			this.textViewChatState = activity.FindViewById<TextView>(Resource.Id.textViewChatState);
			this.textViewNotificationsRadius = activity.FindViewById<TextView>(Resource.Id.textViewNotificationsRadius);
			this.textViewKeywords = activity.FindViewById<TextView>(Resource.Id.textViewKeywords);
			this.textViewHomeLocalization = activity.FindViewById<TextView>(Resource.Id.textViewHomeLocalization);
			this.imgBtnConversations = activity.FindViewById<ImageButton>(Resource.Id.imgBtnConversations);
			this.imgBtnConversations.Click += (s, e) =>
			{
				var intent = new Intent(activity, typeof(ConversationsListActivity));
				activity.StartActivity(intent);
			};


			//conversations
			SetupChatStateView(activity);

			//notifications
			SetupNotificationsStateView(activity);


			this.imgBtnKeywords = activity.FindViewById<ImageButton>(Resource.Id.imgBtnKeywords);
			this.imgBtnKeywords.Click += async (sender, args) =>
			{
				this.progressDialogHelper.ShowProgressDialog("Trwa pobieranie danych");
				var token = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
				try
				{
					var userSelectesKeywordsNames = appSettings.Keywords.Select(k => k.Value).ToList();
					var allKeywords = await this.keyworsService.GetKeywords(token);
					var allKeywordsNames = allKeywords.Select(k => k.Value).ToArray();

					AlertsService.ShowMultiSelectListString(activity, "Wybierz has�a", allKeywordsNames, userSelectesKeywordsNames, selectedItemsNames =>
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
					});

				}
				catch (Exception exc)
				{
					AlertsService.ShowToast(activity, "Wyst�pi� problem z pobraniem danych. Upewnij si�, �e masz dost�p do internetu");
				}
				finally
				{
					this.progressDialogHelper.CloseProgressDialog();
				}

			};



			this.imgBtnHomeLocalization = activity.FindViewById<ImageButton>(Resource.Id.imgBtnHomeLocalization);
			this.imgBtnHomeLocalization.Click += (sender, args) =>
			{
				//gpsService
				var confirmMessage = "Czy na pewno chcesz ustali� aktualn� lokalizacj� i ustawi� j� jako domow�?";
				AlertsService.ShowConfirmDialog(activity, confirmMessage, async () =>
				{
					this.progressDialogHelper.ShowProgressDialog("Trwa ustalanie Twojej aktualnej lokalizacji");
					try
					{
						if (!this.gpsService.CanGetLocation)
						{
							AlertsService.ShowToast(activity, "W��cz gps w ustawieniach");
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
						AlertsService.ShowToast(activity, "Wyst�pi� problem z okre�leniem Twojej lokalizacji. Spr�buj ponownie p�niej");
					}
					finally
					{
						this.progressDialogHelper.CloseProgressDialog();
					}

				});
			};
		}

		private void SetupNotificationsStateView(BaseActivity activity)
		{
			this.notificationsStateSwitch = activity.FindViewById<SwitchCompat>(Resource.Id.switchNotificationsState);
			this.notificationsStateSwitch.Click += (sender, args) =>
			{
				var confirmMessage = String.Format("Czy na pewno chcesz {0} powiadomienia o nowo�ciach?", !notificationsStateSwitch.Checked ? "wy��czy�" : "w��czy�");
				AlertsService.ShowConfirmDialog(activity, confirmMessage, () =>
				{
					if (!notificationsStateSwitch.Checked)
					{
						appSettings.NotificationsDisabled = true;
						activity.StopService(new Intent(ActivityInstancesWhichStartedServices.ActivityWhichStartedNotificationsService, typeof(NewsService)));
					}
					else
					{
						appSettings.NotificationsDisabled = false;
						activity.StartService(new Intent(activity.BaseContext, typeof(NewsService)));
						ActivityInstancesWhichStartedServices.ActivityWhichStartedNotificationsService = activity.BaseContext;
					}

					SetAppSettings(appSettings);
					this.textViewNotificationsState.Text = notificationsStateSwitch.Checked ? "w��czone" : "wy��czone";
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
					var resultRadius = 500;
					var selectedRadius = selectedText.Split(new char[] { ' ' })[0];
					int.TryParse(selectedRadius, out resultRadius);
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
				var confirmMessage = String.Format("Czy na pewno chcesz {0} czat?", !chatStateSwitch.Checked ? "wy��czy�" : "w��czy�");
				AlertsService.ShowConfirmDialog(activity, confirmMessage, () =>
				{
					if (!chatStateSwitch.Checked)
					{
						appSettings.ChatDisabled = true;
						activity.StopService(new Intent(ActivityInstancesWhichStartedServices.ActivityWhichStartedMessengerService, typeof(MessengerService)));
					}
					else
					{
						appSettings.ChatDisabled = false;
						activity.StartService(new Intent(activity.BaseContext, typeof(MessengerService)));
						ActivityInstancesWhichStartedServices.ActivityWhichStartedMessengerService = activity.BaseContext;
					}

					SetAppSettings(appSettings);
					this.textViewChatState.Text = chatStateSwitch.Checked ? "w��czony" : "wy��czony";
				},
				() =>
				{
					this.chatStateSwitch.Checked = !chatStateSwitch.Checked;
				});
			};
		}

		internal void SetupMenu()
		{
			this.appSettings = GetAppSettings();
			SetChatSettings();
			SetNotificationsSettings();
			SetKeywordsSettings(appSettings);
			SetHomeLocationSettings(appSettings);
		}

		private void SetAppSettings(AppSettingsModel appSettings)
		{
			this.sharedPreferencesHelper.SetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS, appSettings);
		}

		private AppSettingsModel GetAppSettings()
		{
			var settingsModel = (AppSettingsModel)this.sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
			if (settingsModel == null)
			{
				settingsModel = new AppSettingsModel();
				settingsModel.LocationSettings.MaxDistance = 500;
				this.sharedPreferencesHelper.SetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS, settingsModel);
			}

			return settingsModel;
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

			this.textViewNotificationsRadius.Text = settingsModel.LocationSettings.MaxDistance < 500 ? String.Format("{0} km", settingsModel.LocationSettings.MaxDistance.ToString()) : "bez ogranicze�";

		}

		private void SetNotificationsSettings()
		{
			if (!appSettings.NotificationsDisabled)
			{
				this.textViewNotificationsState.Text = "w��czone";

				this.notificationsStateSwitch.Checked = true;
			}
			else
			{
				this.textViewNotificationsState.Text = "wy��czone";
				this.notificationsStateSwitch.Checked = false;
			}
		}

		private void SetChatSettings()
		{
			if (appSettings.ChatDisabled)
			{
				this.textViewChatState.Text = "wy��czony";
				this.chatStateSwitch.Checked = false;
			}
			else
			{
				this.textViewChatState.Text = "w��czony";
				this.chatStateSwitch.Checked = true;
			}
		}
	}
}