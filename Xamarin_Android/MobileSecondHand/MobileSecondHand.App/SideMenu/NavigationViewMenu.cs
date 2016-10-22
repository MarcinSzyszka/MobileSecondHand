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

namespace MobileSecondHand.App.SideMenu
{
	public class NavigationViewMenu
	{
		CategoriesSelectingHelper categoriesHelper;
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
		private BaseActivity activity;

		public NavigationViewMenu(BaseActivity activity, SharedPreferencesHelper sharedPreferencesHelper)
		{
			this.activity = activity;
			this.progressDialogHelper = new ProgressDialogHelper(activity);
			this.sharedPreferencesHelper = sharedPreferencesHelper;
			this.gpsService = GpsLocationService.GetServiceInstance(activity);
			this.googleMapsAPIService = new GoogleMapsAPIService();
			this.categoriesHelper = new CategoriesSelectingHelper(activity);
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
				var userSelectesKeywordsNames = appSettings.Keywords.Select(k => k.Value).ToList();
				await this.categoriesHelper.ShowCategoriesListAndMakeAction(userSelectesKeywordsNames, MethodToExecuteAfterCategoriesSelect);
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
							AlertsService.ShowToast(activity, "W³¹cz gps w ustawieniach");
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
						AlertsService.ShowToast(activity, "Wyst¹pi³ problem z okreœleniem Twojej lokalizacji. Spróbuj ponownie póŸniej");
					}
					finally
					{
						this.progressDialogHelper.CloseProgressDialog();
					}

				});
			};
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
						activity.StopService(new Intent(ActivityInstancesWhichStartedServices.ActivityWhichStartedNotificationsService, typeof(NewsService)));
					}
					else
					{
						appSettings.NotificationsDisabled = false;
						activity.StartService(new Intent(activity.BaseContext, typeof(NewsService)));
						ActivityInstancesWhichStartedServices.ActivityWhichStartedNotificationsService = activity.BaseContext;
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
						resultRadius = 500;
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
						activity.StopService(new Intent(ActivityInstancesWhichStartedServices.ActivityWhichStartedMessengerService, typeof(MessengerService)));
					}
					else
					{
						appSettings.ChatDisabled = false;
						activity.StartService(new Intent(activity.BaseContext, typeof(MessengerService)));
						ActivityInstancesWhichStartedServices.ActivityWhichStartedMessengerService = activity.BaseContext;
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
			this.appSettings = SharedPreferencesHelper.GetAppSettings(activity);
			textViewUserName.Text = appSettings.UserName;
			SetChatSettings();
			SetNotificationsSettings();
			SetKeywordsSettings(appSettings);
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
			if (appSettings.ChatDisabled)
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