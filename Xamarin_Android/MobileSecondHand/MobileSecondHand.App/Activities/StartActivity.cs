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
		ProgressDialogHelper progressHelper;
		ISignInService signInService;
		Action<bool> actionToExecuteAfterCloseSettingsDialog;
		private bool settingsAlertIsShow;
		private bool userIsLogged;
		RelativeLayout setUserNameLayout;
		RelativeLayout logoLayout;
		private EditText userNameEditText;
		private Button btnSetNickName;
		private static TokenModel tokenModel;
		private SharedPreferencesHelper preferenceHelper;

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
			progressHelper = new ProgressDialogHelper(this);
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
			btnSetNickName.Click += BtnSetNickName_Click;
		}

		private async void BtnSetNickName_Click(object sender, EventArgs e)
		{
			userNameEditText.Text = userNameEditText.Text.Replace(" ", "");
			if (userNameEditText.Text.Length < 4)
			{
				AlertsService.ShowToast(this, "Nick musi składać się z conajmniej czterech znaków");
			}
			else
			{
				try
				{
					this.progressHelper.ShowProgressDialog("Przetwarzanie danych..");
					bool result = await this.signInService.SetUserName(userNameEditText.Text, GetTokenModel());
					this.progressHelper.CloseProgressDialog();
					if (result)
					{
						this.preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.USER_NAME, userNameEditText.Text);
						StartMainOrLoginActivity(true);
					}
					else
					{
						AlertsService.ShowToast(this, "Podany nick jest już zajęty. Wpisz inny nick.");
					}
				}
				catch (Exception exc)
				{
					AlertsService.ShowToast(this, "Wystąpił błąd połączenia z serwerem.");
					this.progressHelper.CloseProgressDialog();
				}
			}
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
			preferenceHelper = new SharedPreferencesHelper(this);
			var bearerToken = (string)preferenceHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			if (bearerToken != null)
			{
				userIsLogged = await signInService.SignInUserWithBearerToken(GetTokenModel());
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

		private TokenModel GetTokenModel()
		{
			if (tokenModel == null)
			{
				var bearerToken = (string)preferenceHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
				tokenModel = new TokenModel { Token = bearerToken };
			}

			return tokenModel;
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

