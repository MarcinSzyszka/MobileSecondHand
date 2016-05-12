using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Services.Chat;

namespace MobileSecondHand.App.Chat {
	[Service]
	public class MessengerService : Service {
		private SharedPreferencesHelper sharedPreferencesHelper;
		ChatHubClientService chatHubClientService;

		public override IBinder OnBind(Intent intent) {
			//throw new NotImplementedException();
			return null;
		}

		[return: GeneratedEnum]
		public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId) {
			this.sharedPreferencesHelper = new SharedPreferencesHelper(Application.ApplicationContext);
			this.chatHubClientService = new ChatHubClientService();
			DoWork();
			return StartCommandResult.Sticky;
		}

		private void DoWork() {
			var t = new Thread(() => {
				var bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
				this.chatHubClientService.RegisterReceiveMessages(ShowNotification);
				this.chatHubClientService.RegisterInHub(bearerToken);
			}
		);
			t.Start();
		}

		private void ShowNotification(string message) {
			//Toast.MakeText(ApplicationContext, message, ToastLength.Long).Show();
			var nMgr = (NotificationManager)GetSystemService(NotificationService);
			var notification = new Notification(Resource.Drawable.Icon, message);
			notification.Sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
			var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), 0);
			notification.SetLatestEventInfo(this, "Mobile Second Hand", message, pendingIntent);
			nMgr.Notify(0, notification);
		}
	}
}