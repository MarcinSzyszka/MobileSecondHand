using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Hardware.Display;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MobileSecondHand.API.Models.Shared.Chat;
using MobileSecondHand.App.Activities;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.Services.Chat;
using Newtonsoft.Json;

namespace MobileSecondHand.App.Chat
{
	[Service]
	public class MessengerService : Service
	{
		private int timeoutInSeconds;
		private SharedPreferencesHelper sharedPreferencesHelper;
		ChatHubClientService chatHubClientService;
		private string bearerToken;
		private Thread signalRThread;
		private int lastMessageId;
		private Timer timer;
		private PowerManager powerManager;
		private int timerInterval;
		private DisplayManager displayManager;
		private DateTime? lastScreenTurnOffDate;
		private DateTime lastConnectionDate;
		private static int outsidePendingWorks;
		private DateTime notReceivedMessagesCheckedTime;
		public static bool ServiceIsRunning { get; private set; }

		public override IBinder OnBind(Intent intent)
		{
			//throw new NotImplementedException();
			return null;
		}

		public override void OnDestroy()
		{
			this.chatHubClientService.Dispose();
			timer.Dispose();
			ServiceIsRunning = false;
			this.signalRThread.Abort();
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
					timeoutInSeconds = 15;
					timerInterval = 1000 * 5;
					displayManager = (DisplayManager)GetSystemService(Context.DisplayService);
					powerManager = (PowerManager)GetSystemService(Context.PowerService);
					this.sharedPreferencesHelper = new SharedPreferencesHelper(Application.ApplicationContext);
					this.bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
					this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
					lastConnectionDate = DateTime.Now;
					this.chatHubClientService.RegisterReceiveMessages(ShowNotification);
					this.chatHubClientService.RegisterNotReceivedMessagesChecked(SetCheckedNotReceivedMessages);


					timer = new Timer(new TimerCallback(TimerCallBackMethod));
					timer.Change(timerInterval, timerInterval);
				}
		);
			signalRThread.Start();
		}

		private void SetCheckedNotReceivedMessages()
		{
			notReceivedMessagesCheckedTime = DateTime.Now;
		}

		private void TimerCallBackMethod(object state)
		{
			if (IsScreenOff())
			{
				SetLastTurnOffScreenDate();

				if (this.chatHubClientService.IsConnected())
				{
					StopConnectionAfterTimeout();
				}
				else if (lastConnectionDate.AddMinutes(10) < DateTime.Now)
				{
					lastConnectionDate = DateTime.Now;
					this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
				}
			}
			else
			{
				lastConnectionDate = DateTime.Now;
				lastScreenTurnOffDate = null;
				if (!this.chatHubClientService.IsConnected())
				{
					this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
				}
			}
		}

		private void StopConnectionAfterTimeout()
		{
			if (outsidePendingWorks == 0 &&
				((lastConnectionDate.AddSeconds(timeoutInSeconds) < DateTime.Now) && (lastScreenTurnOffDate.HasValue && lastScreenTurnOffDate.Value.AddSeconds(timeoutInSeconds) < DateTime.Now)) ||
				((lastScreenTurnOffDate.HasValue && lastScreenTurnOffDate.Value.AddSeconds(timeoutInSeconds) < DateTime.Now) && (notReceivedMessagesCheckedTime > lastConnectionDate)))
			{
				this.chatHubClientService.StopConnection();
			}
		}

		private void SetLastTurnOffScreenDate()
		{
			if (!lastScreenTurnOffDate.HasValue)
			{
				lastScreenTurnOffDate = DateTime.Now;
			}
		}

		private bool IsScreenOff()
		{

			if (Build.VERSION.SdkInt >= BuildVersionCodes.KitkatWatch)
			{
				return !powerManager.IsInteractive;
			}
			else
			{
				return !powerManager.IsScreenOn;
			}
		}

		private void ShowNotification(string conversationMessage)
		{
			var message = JsonConvert.DeserializeObject<ConversationMessage>(conversationMessage);
			if (lastMessageId == message.Id)
			{
				//kilka po³aczeñ z hubem a header jest unikalny dla wiadomoœci
				return;
			}
			if (!ConversationActivity.ConversationActivityStateModel.IsInForeground || ConversationActivity.ConversationActivityStateModel.ConversationId != message.ConversationId)
			{
				var nMgr = (NotificationManager)GetSystemService(NotificationService);
				var notification = new Notification(Resource.Drawable.logo_icon, "Mobile Second Hand - nowa wiadomoœæ");
				var notificationId = new System.Random().Next(1000);
				notification.Flags = NotificationFlags.AutoCancel;
				notification.Sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
				var intent = new Intent(this, typeof(ConversationActivity));
				var conversationInfoModel = new ConversationInfoModel
				{
					ConversationId = message.ConversationId,
					InterlocutorId = message.SenderId,
					InterlocutorName = message.SenderName,
					InterlocutorPrifileImage = new byte[0]
				};
				intent.PutExtra(ExtrasKeys.CONVERSATION_INFO_MODEL, JsonConvert.SerializeObject(conversationInfoModel));
				var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.CancelCurrent);
				notification.SetLatestEventInfo(this, String.Format("Wiadomoœæ od {0}", message.SenderName), message.MessageContent, pendingIntent);
				nMgr.Notify(conversationInfoModel.ConversationId, notification);
			}
			else
			{
				ConversationActivity.ActivityInstance.AddMessage(message);
			}

			lastMessageId = message.Id;
			this.chatHubClientService.MessageReceived(message.Id);
		}

		public static void AddOutsidePendingWork()
		{
			outsidePendingWorks++;
		}

		public static void RemoveOutsidePendingWork()
		{
			if (outsidePendingWorks > 0)
			{
				outsidePendingWorks--;
			}
		}
	}
}