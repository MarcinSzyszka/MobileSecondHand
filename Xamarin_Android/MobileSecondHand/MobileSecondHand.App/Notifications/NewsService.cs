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

namespace MobileSecondHand.App.Notifications
{
	[Service]
	public class NewsService : Service
	{
		public static bool ServiceIsRunnig { get; private set; }
		public override IBinder OnBind(Intent intent)
		{
			return null;
		}
	}
}