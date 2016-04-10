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

namespace MobileSecondHand.App.Infrastructure {
	public class AlertsService {
		public static void ShowToast(Context context, string message) {
			Toast.MakeText(context, message, ToastLength.Long).Show();
		}

		public static void ShowAlertDialog(Context context, string message) {
			AlertDialog.Builder alert = new AlertDialog.Builder(context);
			alert.SetTitle("Informacja");
			alert.SetMessage(message);
			alert.Show();
		}
	}
}