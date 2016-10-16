using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.OS;
using MobileSecondHand.Services.Authentication;
using Xamarin.Facebook;
using MobileSecondHand.App.Consts;
using System.Threading.Tasks;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Services.Location;
using MobileSecondHand.Models.Exceptions;
using MobileSecondHand.API.Models.Shared.Security;
using Android.Support.V7.App;
using Android.Widget;

namespace MobileSecondHand.App.Activities
{
	[Activity(MainLauncher = true, Icon = "@drawable/logo_icon")]
	public class StartActivity : AppCompatActivity, ISettingWindowCloseListener
	{
		ISignInService signInService;
		Action<bool> actionToExecuteAfterCloseSettingsDialog;
		private bool settingsAlertIsShow;
		private bool userIsLogged;
		RelativeLayout setUserNameLayout;
		RelativeLayout logoLayout;
		private EditText userNameEditText;
		private Button btnSetNickName;

		public StartActivity()
		{
			this.signInService = new SignInService();
		}
		protected override async void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			FacebookSdk.SdkInitialize(this);
			SetFullscreenOptions();
			SetContentView(Resource.Layout.StartActivity);
			SetupViews();
			var gps = new GpsLocationService(this, this);
			if (!gps.CanGetLocation)
			{
				settingsAlertIsShow = true;
				gps.ShowSettingsAlert();

			}



			try
			{
				userIsLogged = await SignInUser();
			}
			catch (UserHasToSetNickNameException exc)
			{
				logoLayout.Visibility = ViewStates.Gone;
				setUserNameLayout.Visibility = ViewStates.Visible;
				return;
			}
			catch (Exception exc)
			{
				AlertsService.ShowAlertDialog(this, "Wystąpił problem z połączeniem z serwerem. Spróbuj ponownie później");
				return;
			}

			if (!settingsAlertIsShow)
			{
				StartMainOrLoginActivity(userIsLogged);
			}
			else
			{
				actionToExecuteAfterCloseSettingsDialog = StartMainOrLoginActivity;
			}
		}

		private void SetupViews()
		{
			logoLayout = FindViewById<RelativeLayout>(Resource.Id.logoLayout);
			setUserNameLayout = FindViewById<RelativeLayout>(Resource.Id.setUserNameLayout);
			userNameEditText = FindViewById<EditText>(Resource.Id.editTextNickName);
			btnSetNickName = FindViewById<Button>(Resource.Id.btnSaveNickName);
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (actionToExecuteAfterCloseSettingsDialog != null)
			{
				actionToExecuteAfterCloseSettingsDialog(userIsLogged);
				actionToExecuteAfterCloseSettingsDialog = null;
			}
		}

		private void StartMainOrLoginActivity(bool userIsLogged)
		{
			var intent = default(Intent);
			if (userIsLogged)
			{
				intent = new Intent(this, typeof(MainActivity));
			}
			else
			{
				intent = new Intent(this, typeof(LoginActivity));
			}
			this.Finish();
			StartActivity(intent);
		}

		private async Task<bool> SignInUser()
		{
			var userIsLogged = false;
			var preferenceHelper = new SharedPreferencesHelper(this);
			var bearerToken = (string)preferenceHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			if (bearerToken != null)
			{
				userIsLogged = await signInService.SignInUserWithBearerToken(new TokenModel { Token = bearerToken });
			}
			if (!userIsLogged)
			{
				if (AccessToken.CurrentAccessToken != null && AccessToken.CurrentAccessToken.Token != null)
				{
					var facebookToken = new FacebookTokenViewModel { FacebookToken = AccessToken.CurrentAccessToken.Token };
					var tokenModel = await signInService.SignInUserWithFacebookToken(facebookToken);
					if (tokenModel != null)
					{
						preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, tokenModel.Token);
						if (tokenModel.UserHasToSetNickName)
						{
							throw new UserHasToSetNickNameException();
						}
						userIsLogged = true;
					}
				}
			}

			return userIsLogged;
		}

		private void SetFullscreenOptions()
		{
			this.Window.RequestFeature(WindowFeatures.NoTitle);
			this.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
		}

		public void OnSettingsWindowClose()
		{
			if (actionToExecuteAfterCloseSettingsDialog != null)
			{
				actionToExecuteAfterCloseSettingsDialog(userIsLogged);
				actionToExecuteAfterCloseSettingsDialog = null;
			}
		}
	}
}

