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
using MobileSecondHand.API.Models.Shared.Consts;
using MobileSecondHand.App.Consts;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.Settings;
using Newtonsoft.Json;

namespace MobileSecondHand.App.Infrastructure
{
	public class SharedPreferencesHelper
	{
		Context context;

		public SharedPreferencesHelper(Context context)
		{
			this.context = context;
		}

		public object GetSharedPreference<T>(string key)
		{
			var preference = PreferenceManager.GetDefaultSharedPreferences(context);
			if (typeof(T) == typeof(int))
			{
				return preference.GetInt(key, 0);
			}
			else if (typeof(T) == typeof(bool))
			{
				return preference.GetBoolean(key, false);
			}
			else if (typeof(T) == typeof(string))
			{
				return preference.GetString(key, null);
			}
			else
			{
				var json = preference.GetString(key, null);
				if (json != null)
				{
					return JsonConvert.DeserializeObject<T>(json);
				}
				else
				{
					return null;
				}

			}
		}

		public void SetSharedPreference<T>(string key, object value)
		{
			var preference = PreferenceManager.GetDefaultSharedPreferences(context);
			var preferencesEditor = preference.Edit();
			if (typeof(T) == typeof(int))
			{
				preferencesEditor.PutInt(key, (int)value);
			}
			else if (typeof(T) == typeof(bool))
			{
				preferencesEditor.PutBoolean(key, (bool)value);
			}
			else if (typeof(T) == typeof(string))
			{
				preferencesEditor.PutString(key, (string)value);
			}
			else
			{
				var json = JsonConvert.SerializeObject(value);
				preferencesEditor.PutString(key, json);
			}
			preferencesEditor.Commit();
		}

		public static AppSettingsModel GetAppSettings(Context context)
		{
			AppSettingsModel settingsModel;

			var appSettingsString = PreferenceManager.GetDefaultSharedPreferences(context).GetString(SharedPreferencesKeys.APP_SETTINGS, null);
			if (appSettingsString != null)
			{
				settingsModel = JsonConvert.DeserializeObject<AppSettingsModel>(appSettingsString);
			}
			else
			{
				settingsModel = new AppSettingsModel();
				settingsModel.LocationSettings.MaxDistance = ValueConsts.MAX_DISTANCE_VALUE;
				appSettingsString = JsonConvert.SerializeObject(settingsModel);
				var preference = PreferenceManager.GetDefaultSharedPreferences(context);
				var preferencesEditor = preference.Edit();
				preferencesEditor.PutString(SharedPreferencesKeys.APP_SETTINGS, appSettingsString);
			}

			return settingsModel;
		}


		public static void SetAppSettings(Context context, AppSettingsModel appSettingsModel)
		{
			var preference = PreferenceManager.GetDefaultSharedPreferences(context);
			var preferencesEditor = preference.Edit();
			var json = JsonConvert.SerializeObject(appSettingsModel);
			preferencesEditor.PutString(SharedPreferencesKeys.APP_SETTINGS, json);
			preferencesEditor.Commit();
		}

		public static void SetUserNameInAppSettings(Context context, string userName)
		{
			var settingsModel = SharedPreferencesHelper.GetAppSettings(context);
			settingsModel.UserName = userName;
			SharedPreferencesHelper.SetAppSettings(context, settingsModel);
		}
	}
}