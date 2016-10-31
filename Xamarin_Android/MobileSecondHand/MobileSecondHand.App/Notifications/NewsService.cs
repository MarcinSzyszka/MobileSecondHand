using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.Services.Advertisements;
using Newtonsoft.Json;

namespace MobileSecondHand.App.Notifications
{
	[Service]
	public class NewsService : Service
	{
		private IAdvertisementItemService advertisementItemService;
		private GpsLocationService gpsLocationService;
		private AdvertisementsSearchModelForNotifications searchModelForNotifications;
		private Thread newsThread;
		private Timer timer;
		private SharedPreferencesHelper sharedPreferencesHelper;

		public static bool ServiceIsRunning { get; internal set; }

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override void OnDestroy()
		{
			ServiceIsRunning = false;
			timer.Dispose();
			newsThread.Abort();
			base.OnDestroy();
		}

		[return: GeneratedEnum]
		public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
		{
			ServiceIsRunning = true;
			searchModelForNotifications = new AdvertisementsSearchModelForNotifications();
			this.sharedPreferencesHelper = new SharedPreferencesHelper(Application.ApplicationContext);
			var bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			this.advertisementItemService = new AdvertisementItemService(bearerToken);
			this.gpsLocationService = new GpsLocationService(Application.ApplicationContext, null);

			DoWork();

			return StartCommandResult.Sticky;
		}

		private void DoWork()
		{
			this.newsThread = new Thread(() =>
			{
				timer = new Timer(new TimerCallback(TimerCallBackMethod));
				timer.Change(0, 1000 * 60 * 1);
			}
		);
			newsThread.Start();
		}

		private async void TimerCallBackMethod(object state)
		{
			await CheckNewAdvertisementsAroundUserCurrentLocation();
			await CheckNewAdvertisementsAroundUserHomeLocation();
		}

		private async Task CheckNewAdvertisementsAroundUserHomeLocation()
		{
			var appsettings = (AppSettingsModel)this.sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);

			if (appsettings == null && appsettings.LocationSettings.Latitude == 0.00D)
			{
				//nic nie robie
				return;
			}
			SetSearchModel(appsettings);
			var areThereNewAdvertisements = await this.advertisementItemService.CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(searchModelForNotifications);
			if (areThereNewAdvertisements)
			{
				NotifyUserAboutNewAdvertisements(AdvertisementsKind.AdvertisementsArounUserHomeLocation);
			}
		}

	
		private async Task CheckNewAdvertisementsAroundUserCurrentLocation()
		{
			var appsettings = (AppSettingsModel)this.sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
			SetSearchModel(appsettings);
			try
			{
				searchModelForNotifications.CoordinatesModels = gpsLocationService.GetCoordinatesModel();
			}
			catch (Exception)
			{
				return;
				//nic nie robiê
			}

			var areThereNewAdvertisements = await this.advertisementItemService.CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(searchModelForNotifications);
			if (areThereNewAdvertisements)
			{
				NotifyUserAboutNewAdvertisements(AdvertisementsKind.AdvertisementsAroundUserCurrentLocation);
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
			var nMgr = (NotificationManager)GetSystemService(NotificationService);
			var notification = new Notification(Resource.Drawable.Icon,  "Mobile Second Hand");
			notification.Flags = NotificationFlags.AutoCancel;
			notification.Sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
			var intent = new Intent(this, typeof(MainActivity));
			intent.PutExtra(ExtrasKeys.NEW_ADVERTISEMENT_KIND, JsonConvert.SerializeObject(advertisementsKind));
			var pendingIntent = PendingIntent.GetActivity(Application.ApplicationContext, 0, intent, 0);
			notification.SetLatestEventInfo(Application.ApplicationContext, "Nowe og³oszenia", message, pendingIntent);
			nMgr.Notify(0, notification);
		}

		private string GetMessage(AdvertisementsKind advertisementsKind)
		{
			if (advertisementsKind == AdvertisementsKind.AdvertisementsAroundUserCurrentLocation)
			{
				return "Dodano nowe og³oszenia w obrêbie Twojej aktualnej lokalizacji";
			}
			else
			{
				return "Dodano nowe og³oszenia w obrêbie Twojej domowej lokalizacji";
			}
		}
	}
}