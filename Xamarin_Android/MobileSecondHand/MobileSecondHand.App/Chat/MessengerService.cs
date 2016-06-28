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
using MobileSecondHand.Models.Chat;
using MobileSecondHand.Services.Chat;

namespace MobileSecondHand.App.Chat
{
	[Service]
	public class MessengerService : Service
	{
		private SharedPreferencesHelper sharedPreferencesHelper;
		ChatHubClientService chatHubClientService;
		bool serviceIsRunnig;

		public override IBinder OnBind(Intent intent)
		{
			//throw new NotImplementedException();
			return null;
		}

		[return: GeneratedEnum]
		public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
		{
			if (!serviceIsRunnig)
			{
				serviceIsRunnig = true;
				this.sharedPreferencesHelper = new SharedPreferencesHelper(Application.ApplicationContext);
				var bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
				this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
				DoWork();
			}

			return StartCommandResult.Sticky;
		}

		private void DoWork()
		{
			var t = new Thread(() =>
			{
				this.chatHubClientService.RegisterReceiveMessages(ShowNotification);
			}
		);
			t.Start();
		}

		private void ShowNotification(string messageContent, string messageHeader, string conversationId, string senderId)
		{
			if (!ConversationActivity.IsInForeground)
			{
				var nMgr = (NotificationManager)GetSystemService(NotificationService);
				var notification = new Notification(Resource.Drawable.Icon, messageContent);
				notification.Flags = NotificationFlags.AutoCancel;
				notification.Sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
				var intent = new Intent(this, typeof(ConversationActivity));
				var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
				notification.SetLatestEventInfo(this, "Mobile Second Hand", messageContent, pendingIntent);
				nMgr.Notify(0, notification);
			}
			else
			{
				var message = new ConversationMessage();
				message.ConversationId = int.Parse(conversationId);
				message.MessageContent = messageContent;
				message.MessageHeader = messageHeader;
				ConversationActivity.ActivityInstance.AddMessage(message);
			}

		}
	}
}