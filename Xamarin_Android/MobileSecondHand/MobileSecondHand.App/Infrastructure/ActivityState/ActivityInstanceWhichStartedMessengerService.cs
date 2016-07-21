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
	public class ActivityInstanceWhichStartedMessengerService
	{
		public static BaseActivity Activity { get; set; }
	}
}