using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private string bearerToken;
		private Thread signalRThread;
		private string lastHeader;

		public static bool ServiceIsRunning { get; private set; }

		public override IBinder OnBind(Intent intent)
		{
			//throw new NotImplementedException();
			return null;
		}

		public override void OnDestroy()
		{
			ServiceIsRunning = false;
			base.OnDestroy();
		}

		[return: GeneratedEnum]
		public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
		{
			ServiceIsRunning = true;
			
			DoWork();

			return StartCommandResult.Sticky;
		}

		private void DoWork()
		{
			this.signalRThread = new Thread(() =>
			{
				this.sharedPreferencesHelper = new SharedPreferencesHelper(Application.ApplicationContext);
				this.bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
				this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
				this.chatHubClientService.RegisterReceiveMessages(ShowNotification);

				var timer = new Timer(new TimerCallback(TimerCallBackMethod));
				timer.Change(0, 5000);
			}
		);
			signalRThread.Start();
		}

		private void TimerCallBackMethod(object state)
		{
			if (!this.chatHubClientService.IsConnected())
			{
				this.chatHubClientService.Reconnect();

			}
		}

		private void ShowNotification(string messageContent, string messageHeader, string conversationId, string senderId)
		{
			if (lastHeader == messageHeader)
			{
				//kilka po³aczeñ z hubem a header jest unikalny dla wiadomoœci
				return;
			}
			if (!ConversationActivity.ConversationActivityStateModel.IsInForeground || ConversationActivity.ConversationActivityStateModel.ConversationId != int.Parse(conversationId))
			{
				var nMgr = (NotificationManager)GetSystemService(NotificationService);
				var notification = new Notification(Resource.Drawable.Icon, messageContent);
				notification.Flags = NotificationFlags.AutoCancel;
				notification.Sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
				var intent = new Intent(this, typeof(ConversationActivity));
				intent.PutExtra(ExtrasKeys.CONVERSATION_ID, int.Parse(conversationId));
				intent.PutExtra(ExtrasKeys.ADDRESSEE_ID, senderId);
				var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
				notification.SetLatestEventInfo(this, "Nowa wiadomoœæ", messageContent, pendingIntent);
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

			lastHeader = messageHeader;
		}
	}
}