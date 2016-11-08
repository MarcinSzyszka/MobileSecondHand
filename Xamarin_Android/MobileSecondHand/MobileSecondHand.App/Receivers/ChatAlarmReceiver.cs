using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Advertisements;
using Newtonsoft.Json;

namespace MobileSecondHand.App.Receivers
{
	[BroadcastReceiver]
	public class WakeUpAlarmReceiver : BroadcastReceiver
	{
		private static PendingIntent pendingIntent;
		bool repeatAlarmIsWorking;
		private Context context;
		private AdvertisementsSearchModelForNotifications searchModelForNotifications;
		private AppSettingsModel appsettings;
		private AdvertisementItemService advertisementItemService;
		private GpsLocationService gpsLocationService;

		public override async void OnReceive(Context context, Intent intent)
		{
			this.context = context.ApplicationContext;
			UpdateAlarm(context);
			this.appsettings = SharedPreferencesHelper.GetAppSettings(this.context);
			if (appsettings.NotificationsDisabled)
			{
				return;
			}
			searchModelForNotifications = new AdvertisementsSearchModelForNotifications();
			var sharedPreferencesHelper = new SharedPreferencesHelper(context.ApplicationContext);
			var bearerToken = (string)sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			this.appsettings = SharedPreferencesHelper.GetAppSettings(this.context);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.gpsLocationService = new GpsLocationService(context.ApplicationContext, null);
			var coordinates = await CheckNewAdvertisementsAroundUserCurrentLocation();
			await CheckNewAdvertisementsAroundUserHomeLocation(coordinates);
		}


		public static void SetWakeUpAlarmRepeating(Context context)
		{
			AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
			if (pendingIntent != null)
			{
				am.Cancel(pendingIntent);
				pendingIntent.Cancel();
			}
			Intent intent = new Intent(context, typeof(WakeUpAlarmReceiver));
			pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);
			am.SetInexactRepeating(AlarmType.ElapsedRealtimeWakeup, 1000 * 60 * 2, AlarmManager.IntervalHour, pendingIntent);
		}

		public static void SetWakeUpAlarmOnce(Context context, DateTime date)
		{
			AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
			Intent intent = new Intent(context, typeof(WakeUpAlarmReceiver));
			pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);
			Calendar calendar = Calendar.Instance;
			calendar.Set(date.Year, date.Month, date.Day, date.Hour, date.Minute);
			am.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
		}


		private void UpdateAlarm(Context context)
		{
			if (DateTime.Now.Hour >= 22 || DateTime.Now.Hour <= 5)
			{
				repeatAlarmIsWorking = false;
				var dateTimeTomorrow = DateTime.Now.AddDays(1);
				AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
				am.Cancel(pendingIntent);
				pendingIntent.Cancel();
				WakeUpAlarmReceiver.SetWakeUpAlarmOnce(context, new DateTime(dateTimeTomorrow.Year, dateTimeTomorrow.Month, dateTimeTomorrow.Day, 6, 0, 0));
			}
			else if (repeatAlarmIsWorking == false)
			{
				repeatAlarmIsWorking = true;
				SetWakeUpAlarmRepeating(context);
			}
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
				//nic nie robi�
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
			var notification = new Notification(Resource.Drawable.logo_icon, "Mobile Second Hand - nowo�ci");
			notification.Flags = NotificationFlags.AutoCancel;
			notification.Sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
			var intent = new Intent(context, typeof(MainActivity));
			intent.PutExtra(ExtrasKeys.NEW_ADVERTISEMENT_KIND, JsonConvert.SerializeObject(advertisementsKind));
			var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
			notification.SetLatestEventInfo(context, "Nowe og�oszenia", message, pendingIntent);
			nMgr.Notify(0, notification);
		}

		private string GetMessage(AdvertisementsKind advertisementsKind)
		{
			if (advertisementsKind == AdvertisementsKind.AdvertisementsAroundUserCurrentLocation)
			{
				return "w obr�bie Twojej aktualnej lokalizacji";
			}
			else
			{
				return "w obr�bie Twojej domowej lokalizacji";
			}
		}
	}
}