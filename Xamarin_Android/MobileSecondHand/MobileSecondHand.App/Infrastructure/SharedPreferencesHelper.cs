using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileSecondHand.App.Infrastructure {
	public class SharedPreferencesHelper {
		Context context;

		public SharedPreferencesHelper(Context context) {
			this.context = context;
		}

		public object GetSharedPreference<T>(string key) {
			var preference = PreferenceManager.GetDefaultSharedPreferences(context);
			if (typeof(T) == typeof(int)) {
				return preference.GetInt(key, 0);
			} else if (typeof(T) == typeof(bool)) {
				return preference.GetBoolean(key, false);
			}
			else if (typeof(T) == typeof(string)) {
				return preference.GetString(key, null);
			}
			else {
				return default(T);
			}
		}

		public void SetSharedPreference<T>(string key, object value) {
			var preference = PreferenceManager.GetDefaultSharedPreferences(context);
			var preferencesEditor = preference.Edit();
			if (typeof(T) == typeof(int)) {
				preferencesEditor.PutInt(key, (int)value);
			}
			else if (typeof(T) == typeof(bool)) {
				preferencesEditor.PutBoolean(key, (bool)value);
			}
			else if (typeof(T) == typeof(string)) {
				preferencesEditor.PutString(key, (string)value);
			}
			preferencesEditor.Commit();
		}
	}
}