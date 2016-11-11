using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.App.Notifications;
using MobileSecondHand.Models.Settings;

namespace MobileSecondHand.App.Receivers
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] { Intent.ActionBootCompleted })]
	public class ServicesLauncher : BroadcastReceiver
	{
		SharedPreferencesHelper sharedPreferencesHelper;
		public override void OnReceive(Context context, Intent intent)
		{
			this.sharedPreferencesHelper = new SharedPreferencesHelper(context);
			var settingsModel = (AppSettingsModel)this.sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
			if (!settingsModel.ChatDisabled && !MessengerService.ServiceIsRunning)
			{
				context.StartService(new Intent(context.ApplicationContext, typeof(MessengerService)));
			}
			WakeUpAlarmReceiver.SetWakeUpAlarmRepeating(context, 1000 * 60 * 1);
		}
	}
}