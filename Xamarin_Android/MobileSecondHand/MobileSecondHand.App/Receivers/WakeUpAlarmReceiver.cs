using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Advertisements;
using MobileSecondHand.Services.Chat;
using Newtonsoft.Json;

namespace MobileSecondHand.App.Receivers
{
	[BroadcastReceiver]
	public class WakeUpAlarmReceiver : BroadcastReceiver
	{
		static PendingIntent pendingIntent;
		Context context;
		AdvertisementsSearchModelForNotifications searchModelForNotifications;
		AppSettingsModel appsettings;
		AdvertisementItemService advertisementItemService;
		GpsLocationService gpsLocationService;
		PowerManager.WakeLock _wakeLock;
		Action checkingNewAdvertsAction;
		System.Threading.Timer timer;
		int timerTick;
		string bearerToken;
		ChatHubClientService chatHubServiceInstance;
		bool checkingNewAdvertsFinished;
		private int timerInterval;

		public override void OnReceive(Context context, Intent intent)
		{
			bearerToken = SharedPreferencesHelper.GetBearerToken(context);
			if (bearerToken == null)
			{
				return;
			}
			this.context = context.ApplicationContext;
			var powerManager = (PowerManager)context.GetSystemService(Context.PowerService);
			_wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "MSH");
			_wakeLock.Acquire();
			timerTick = 1;
			timerInterval = 1000 * 10;
			timer = new System.Threading.Timer(new TimerCallback(TimerCallBackMethod));
			timer.Change(timerInterval, timerInterval);

			this.appsettings = SharedPreferencesHelper.GetAppSettings(this.context);
			if (!appsettings.ChatDisabled)
			{
				chatHubServiceInstance = ChatHubClientService.GetServiceInstance(bearerToken);
			}
			if (!appsettings.NotificationsDisabled)
			{
				SetCheckingNewAdvertisementsAction(context);
				checkingNewAdvertsAction();
			}
			else
			{
				checkingNewAdvertsFinished = true;
			}
		}

		private void TimerCallBackMethod(object state)
		{
			if (checkingNewAdvertsFinished || timerTick == 12)//timerTick == 12 == 2 min
			{
				if (appsettings.ChatDisabled || chatHubServiceInstance.IsConnected())
				{
					timer.Dispose();
					_wakeLock.Release();
				}
			}
			timerTick++;
		}

		private void SetCheckingNewAdvertisementsAction(Context context)
		{
			checkingNewAdvertsAction = async () =>
			{
				try
				{
					searchModelForNotifications = new AdvertisementsSearchModelForNotifications();
					this.appsettings = SharedPreferencesHelper.GetAppSettings(this.context);
					this.advertisementItemService = new AdvertisementItemService(bearerToken);
					this.gpsLocationService = new GpsLocationService(context.ApplicationContext, null);
					var coordinates = await CheckNewAdvertisementsAroundUserCurrentLocation();
					await CheckNewAdvertisementsAroundUserHomeLocation(coordinates);
				}
				catch { }
				finally
				{
					checkingNewAdvertsFinished = true;
				}
			};
		}

		public static void CancelWakeUpAlarmRepeating(Context context)
		{
			AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
			if (pendingIntent != null)
			{
				am.Cancel(pendingIntent);
				pendingIntent.Cancel();
			}
		}

		public static void SetWakeUpAlarmRepeating(Context context, long firstFireAfterMIliseconds)
		{
			WakeUpAlarmReceiver.CancelWakeUpAlarmRepeating(context);
			AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
			Intent intent = new Intent(context, typeof(WakeUpAlarmReceiver));
			pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);
			am.SetInexactRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + firstFireAfterMIliseconds, AlarmManager.IntervalHour, pendingIntent);
		}

		private bool LocationsAreSimiliar(CoordinatesForAdvertisementsModel currentLocationCoordinates, CoordinatesForAdvertisementsModel homeLocationCoordinates)
		{
			double positiveDifferenceKilometers = 0.0111 * 2;
			double negativeDifferenceKilometers = positiveDifferenceKilometers - (positiveDifferenceKilometers * 2);
			var latDifference = currentLocationCoordinates.Latitude - homeLocationCoordinates.Latitude;
			var lonDifference = currentLocationCoordinates.Longitude - homeLocationCoordinates.Longitude;
			if ((latDifference > 0 && (latDifference > positiveDifferenceKilometers)) ||
				(latDifference < 0 && (latDifference < negativeDifferenceKilometers)) ||
				(lonDifference > 0 && (lonDifference > positiveDifferenceKilometers)) ||
				(lonDifference < 0 && (lonDifference < negativeDifferenceKilometers)))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private async Task<CoordinatesForAdvertisementsModel> CheckNewAdvertisementsAroundUserCurrentLocation()
		{
			SetSearchModel(appsettings);
			try
			{
				searchModelForNotifications.CoordinatesModels = gpsLocationService.GetCoordinatesModel();
			}
			catch
			{
				return new CoordinatesForAdvertisementsModel();
				//nic nie robiê
			}

			var areThereNewAdvertisements = await this.advertisementItemService.CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(searchModelForNotifications);
			if (areThereNewAdvertisements)
			{
				NotifyUserAboutNewAdvertisements(AdvertisementsKind.AdvertisementsAroundUserCurrentLocation);
			}

			return searchModelForNotifications.CoordinatesModels;
		}

		private async Task CheckNewAdvertisementsAroundUserHomeLocation(CoordinatesForAdvertisementsModel currentLocationCoordinates)
		{
			if (appsettings == null && appsettings.LocationSettings.Latitude == 0.00D)
			{
				//nic nie robie
				return;
			}
			SetSearchModel(appsettings);
			var areThereNewAdvertisements = await this.advertisementItemService.CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(searchModelForNotifications);
			if (areThereNewAdvertisements)
			{
				if (!LocationsAreSimiliar(currentLocationCoordinates, appsettings.LocationSettings))
				{
					NotifyUserAboutNewAdvertisements(AdvertisementsKind.AdvertisementsArounUserHomeLocation);
				}
			}
		}

		private void SetSearchModel(AppSettingsModel appsettings)
		{
			searchModelForNotifications.CoordinatesModels = appsettings.LocationSettings;
			searchModelForNotifications.Sizes = appsettings.Sizes;
			searchModelForNotifications.CategoriesIds = appsettings.Keywords.Keys.ToList();
		}


		private void NotifyUserAboutNewAdvertisements(AdvertisementsKind advertisementsKind)
		{
			var message = GetMessage(advertisementsKind);
			var nMgr = (NotificationManager)context.GetSystemService(Context.NotificationService);
			var notification = new Notification(Resource.Drawable.logo_icon, "Mobile Second Hand - nowoœci");
			var notificationId = new System.Random().Next(1000);
			notification.Flags = NotificationFlags.AutoCancel;
			notification.Sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
			var intent = new Intent(context, typeof(MainActivity));
			intent.PutExtra(ExtrasKeys.NEW_ADVERTISEMENT_KIND, JsonConvert.SerializeObject(advertisementsKind));
			var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
			notification.SetLatestEventInfo(context, "Nowe og³oszenia", message, pendingIntent);
			nMgr.Notify(notificationId, notification);
		}

		private string GetMessage(AdvertisementsKind advertisementsKind)
		{
			if (advertisementsKind == AdvertisementsKind.AdvertisementsAroundUserCurrentLocation)
			{
				return "w obrêbie Twojej aktualnej lokalizacji";
			}
			else
			{
				return "w obrêbie Twojej domowej lokalizacji";
			}
		}
	}
}