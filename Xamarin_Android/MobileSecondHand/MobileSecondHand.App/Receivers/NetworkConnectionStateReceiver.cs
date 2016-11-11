using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Android.Net.NetworkInfo;

namespace MobileSecondHand.App.Receivers
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] { "android.net.conn.CONNECTIVITY_CHANGE" })]
	public class NetworkConnectionStateReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
			NetworkInfo activeNetwork = cm.ActiveNetworkInfo;
			var isConnected = activeNetwork != null && activeNetwork.IsConnected;
			if (!isConnected)
			{
				WakeUpAlarmReceiver.CancelWakeUpAlarmRepeating(context);
			}
			else
			{
				//first fire after 5 sec
				WakeUpAlarmReceiver.SetWakeUpAlarmRepeating(context, 1000 * 10);
			}
		}
	}
}