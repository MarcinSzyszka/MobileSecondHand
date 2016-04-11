using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using MobileSecondHand.Services.Authentication;
using Xamarin.Facebook;
using Android.Preferences;
using MobileSecondHand.App.Consts;
using System.Threading.Tasks;
using MobileSecondHand.Models.Security;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Services.Location;

namespace MobileSecondHand.App {
	[Activity(MainLauncher = true, Icon = "@drawable/icon")]
	public class StartActivity : Activity, ISettingWindowCloseListener {
		ISignInService signInService;
		Action<bool> actionToExecuteAfterCloseSettingsDialog;
		private bool settingsAlertIsShow;
		private bool userIsLogged;

		public StartActivity() {
			this.signInService = new SignInService();
		}
		protected override async void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			FacebookSdk.SdkInitialize(this);
			SetFullscreenOptions();
			SetContentView(Resource.Layout.StartActivity);
			var gps = new GpsLocationService(this, this);
			if (!gps.CanGetLocation) { 
				settingsAlertIsShow = true;
				gps.ShowSettingsAlert();
				
			}
			userIsLogged = await SignInUser();
			if (!settingsAlertIsShow) {
				StartMainOrLoginActivity(userIsLogged);
			}
			else {
				actionToExecuteAfterCloseSettingsDialog = StartMainOrLoginActivity;
			}
		}

		protected override void OnResume() {
			base.OnResume();
			if (actionToExecuteAfterCloseSettingsDialog != null) {
				actionToExecuteAfterCloseSettingsDialog(userIsLogged);
				actionToExecuteAfterCloseSettingsDialog = null;
			}
		}

		private void StartMainOrLoginActivity(bool userIsLogged) {
			var intent = default(Intent);
			if (userIsLogged) {
				intent = new Intent(this, typeof(MainActivity));
			}
			else {
				intent = new Intent(this, typeof(LoginActivity));
			}
			this.Finish();
			StartActivity(intent);
		}

		private async Task<bool> SignInUser() {
			var userIsLogged = false;
			var preferenceHelper = new SharedPreferencesHelper(this);
			var bearerToken = (string)preferenceHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			if (bearerToken != null) {
				userIsLogged = await signInService.SignInUserWithBearerToken(new TokenModel { Token = bearerToken });
			}
			if (!userIsLogged) {
				if (AccessToken.CurrentAccessToken != null && AccessToken.CurrentAccessToken.Token != null) {
					var facebookToken = new FacebookTokenViewModel { FacebookToken = AccessToken.CurrentAccessToken.Token };
					var tokenModel = await signInService.SignInUserWithFacebookToken(facebookToken);
					if (tokenModel != null) {
						preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, tokenModel.Token);
						userIsLogged = true;
					}
				}
			}

			return userIsLogged;
		}

		private void SetFullscreenOptions() {
			this.Window.RequestFeature(WindowFeatures.NoTitle);
			this.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
		}

		public void OnSettingsWindowClose() {
			if (actionToExecuteAfterCloseSettingsDialog != null) {
				actionToExecuteAfterCloseSettingsDialog(userIsLogged);
				actionToExecuteAfterCloseSettingsDialog = null;
			}
		}
	}
}

