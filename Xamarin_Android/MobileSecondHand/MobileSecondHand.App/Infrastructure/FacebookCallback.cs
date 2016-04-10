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
using Xamarin.Facebook;

namespace MobileSecondHand.App.Infrastructure {
	public class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object {
		public Action HandleCancel { get; set; }
		public Action<FacebookException> HandleError { get; set; }
		public Action<TResult> HandleSuccess { get; set; }

		public void OnCancel() {
			var c = HandleCancel;
			if (c != null)
				c();
		}

		public void OnError(FacebookException error) {
			var c = HandleError;
			if (c != null)
				c(error);
		}

		public void OnSuccess(Java.Lang.Object result) {
			var c = HandleSuccess;
			if (c != null)
				c(result.JavaCast<TResult>());
		}
	}
}