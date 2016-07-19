using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Security;
using MobileSecondHand.Services.Authentication;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "LoginActivity")]
	public class LoginActivity : Activity {
		private ICallbackManager callbackManager;
		private ISignInService signInService;
		private SharedPreferencesHelper preferenceHelper;
		private EditText emailInput;
		private EditText passwordInput;
		private Button buttonLogin;
		private Button buttonRegistration;
		private View focusView;

		public LoginActivity() {
			this.signInService = new SignInService();
			this.preferenceHelper = new SharedPreferencesHelper(this);
		}
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			//FacebookSdk.SdkInitialize(this.ApplicationContext);

			SetContentView(Resource.Layout.LoginActivity);

			SetupViews();
			// Create your application here
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
			base.OnActivityResult(requestCode, resultCode, data);

			callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
		}

		private void SetupViews() {
			SetupFacebookLogin();
			SetupStandardLogin();
			SetupGoToRegistration();
		}

		private void SetupGoToRegistration() {
			buttonRegistration = FindViewById<Button>(Resource.Id.goToRegistrationButton);
			buttonRegistration.Click += ButtonRegistration_Click;
		}

		private void ButtonRegistration_Click(object sender, EventArgs e) {
			var registerIntent = new Intent(this, typeof(RegisterActivity));
			StartActivity(registerIntent);
		}

		private void SetupStandardLogin() {
			emailInput = FindViewById<EditText>(Resource.Id.emailInputLogin);
			passwordInput = FindViewById<EditText>(Resource.Id.inputPasswordLogin);
			buttonLogin = FindViewById<Button>(Resource.Id.buttonLoginStandard);

			buttonLogin.Click += async (s, e) => await ButtonLogin_Click(s, e);
		}

		private async Task ButtonLogin_Click(object sender, EventArgs e) {
			if (loginFormIsValid()) {
				await LoginUser();
			}
			else {
				focusView.RequestFocus();
			}
		}

		private async Task LoginUser() {
			var loginModel = new LoginModel {
				Email = emailInput.Text,
				Password = passwordInput.Text
			};
			var tokenModel = await this.signInService.SignInUserStandard(loginModel);
			if (tokenModel != null) {
				preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, tokenModel.Token);
				GoToMainActivity();
			}
			else {
				AlertsService.ShowToast(this, "Coœ posz³o nie tak na serwerze!");
			}
		}

		private bool loginFormIsValid() {
			var formValidator = new FormValidator();
			return formValidator.IsLoginFormValidate(emailInput, passwordInput, ref focusView);
		}

		private void SetupFacebookLogin() {
			callbackManager = CallbackManagerFactory.Create();
			LoginButton facebookLoginBtn = FindViewById<LoginButton>(Resource.Id.facebookLoginBtn);
			facebookLoginBtn.SetReadPermissions("public_profile", "email");
			var loginCallback = new FacebookCallback<LoginResult> {
				HandleSuccess = async loginResult => {
					AlertsService.ShowToast(this, "Facebook zwróci³ token");
					var userIsLogged = await LoginWithFacebook();
					if (userIsLogged) {
						GoToMainActivity();
					}
					else {
						AlertsService.ShowToast(this, "Facebook zwróci³ token, ale coœ posz³o nie tak z logowaniem na serwerze");
					}
				},
				HandleCancel = () => {
					AlertsService.ShowToast(this, "Przerwano logowanie z facebookiem");
				},
				HandleError = loginError => {
					AlertsService.ShowToast(this, "Wyst¹pi³ b³¹ podczas logowanie z facebookiem");
				}
			};
			facebookLoginBtn.RegisterCallback(this.callbackManager, loginCallback);
		}

		private void GoToMainActivity() {
			var mainIntent = new Intent(this, typeof(MainActivity));
			this.Finish();
			StartActivity(mainIntent);
		}

		private async Task<bool> LoginWithFacebook() {
			var userIsLogged = false;
			var fbTokenViewModel = new FacebookTokenViewModel { FacebookToken = AccessToken.CurrentAccessToken.Token };
			var bearerToken = await this.signInService.SignInUserWithFacebookToken(fbTokenViewModel);
			if (bearerToken != null) {
				preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, bearerToken.Token);
				userIsLogged = true;
			}
			return userIsLogged;
		}
	}
}