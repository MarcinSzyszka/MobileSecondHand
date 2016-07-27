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
using MobileSecondHand.App.Activities;

namespace MobileSecondHand.App.Infrastructure.ActivityState
{
	public class ActivityInstancesWhichStartedServices
	{
		public static BaseActivity ActivityWhichStartedMessengerService { get; set; }
		public static BaseActivity ActivityWhichStartedNotificationsService { get; set; }
	}
}