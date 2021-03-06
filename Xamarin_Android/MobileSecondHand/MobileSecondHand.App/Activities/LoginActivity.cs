using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Text.Method;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Services.Authentication;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]

namespace MobileSecondHand.App.Activities
{
	[Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class LoginActivity : AppCompatActivity
	{
		private ICallbackManager callbackManager;
		private ISignInService signInService;
		private SharedPreferencesHelper preferenceHelper;
		private EditText emailInput;
		private EditText passwordInput;
		private Button buttonLogin;
		private Button buttonRegistration;
		private View focusView;
		private ProgressDialogHelper progress;
		TextView textViewReg;
		private LoginButton facebookLoginBtn;

		public LoginActivity()
		{
			this.signInService = new SignInService();
			this.preferenceHelper = new SharedPreferencesHelper(this);
		}
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.LoginActivity);

			SetupViews();
			// Create your application here
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
		}

		private void SetupViews()
		{
			progress = new ProgressDialogHelper(this);
			SetupFacebookLogin();
			SetupAcceptCheckbox();
			SetupStandardLogin();
			SetupGoToRegistration();
		}

		private void SetupAcceptCheckbox()
		{
			textViewReg = FindViewById<TextView>(Resource.Id.textView2);
			textViewReg.TextFormatted = Html.FromHtml("Loguj�c si� przez facebook akceptujesz <a href='" + WebApiConsts.WEB_API_URL + "file/reg'>Regulamin</a> oraz <a href='" + WebApiConsts.WEB_API_URL + "file/privpolicy'>Polityk� Prywatno�ci</a>.");
			textViewReg.MovementMethod = LinkMovementMethod.Instance;
		}

		private void SetupGoToRegistration()
		{
			buttonRegistration = FindViewById<Button>(Resource.Id.goToRegistrationButton);
			buttonRegistration.Click += ButtonRegistration_Click;
		}

		private void ButtonRegistration_Click(object sender, EventArgs e)
		{
			var registerIntent = new Intent(this, typeof(RegisterActivity));
			StartActivity(registerIntent);
			Finish();
		}

		private void SetupStandardLogin()
		{
			emailInput = FindViewById<EditText>(Resource.Id.emailInputLogin);
			passwordInput = FindViewById<EditText>(Resource.Id.inputPasswordLogin);
			buttonLogin = FindViewById<Button>(Resource.Id.buttonLoginStandard);

			buttonLogin.Click += async (s, e) => await ButtonLogin_Click(s, e);
		}

		private async Task ButtonLogin_Click(object sender, EventArgs e)
		{
			if (loginFormIsValid())
			{
				progress.ShowProgressDialog("Trwa logowanie... Prosz� czeka�");
				await LoginUser();
				progress.CloseProgressDialog();
			}
			else
			{
				focusView.RequestFocus();
			}
		}

		private async Task LoginUser()
		{
			var loginModel = new LoginModel
			{
				Email = emailInput.Text,
				Password = passwordInput.Text
			};
			var tokenModel = await this.signInService.SignInUserStandard(loginModel);
			if (tokenModel != null)
			{
				preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, tokenModel.Token);
				if (tokenModel.UserHasToSetNickName)
				{
					GoToActivity(new Intent(this, typeof(StartActivity)));
				}
				else
				{
					SetUserNameInAppSettings(tokenModel.UserName);
					GoToActivity(new Intent(this, typeof(MainActivity)));
				}

			}
			else
			{
				AlertsService.ShowLongToast(this, "Podany email lub has�o s� b��dne");
			}
		}

		private bool loginFormIsValid()
		{
			var formValidator = new FormValidator();
			return formValidator.IsLoginFormValidate(emailInput, passwordInput, ref focusView);
		}

		private void SetupFacebookLogin()
		{
			callbackManager = CallbackManagerFactory.Create();
			facebookLoginBtn = FindViewById<LoginButton>(Resource.Id.facebookLoginBtn);
			facebookLoginBtn.SetReadPermissions("public_profile", "email");
			var loginCallback = new FacebookCallback<LoginResult>
			{
				HandleSuccess = async loginResult =>
				{
					progress.ShowProgressDialog("Trwa logowanie w aplikacji... Prosz� czeka�");
					var token = await LoginWithFacebook();
					if (token != null)
					{
						progress.CloseProgressDialog();
						if (token.UserHasToSetNickName)
						{
							GoToActivity(new Intent(this, typeof(StartActivity)));
						}
						else
						{
							SetUserNameInAppSettings(token.UserName);
							GoToActivity(new Intent(this, typeof(MainActivity)));
						}

					}
					else
					{
						progress.CloseProgressDialog();
						AlertsService.ShowLongToast(this, "Facebook zwr�ci� token, ale co� posz�o nie tak z logowaniem na serwerze");
					}
				},
				HandleCancel = () =>
				{
					progress.CloseProgressDialog();
					AlertsService.ShowLongToast(this, "Przerwano logowanie z facebookiem");
				},
				HandleError = loginError =>
				{
					progress.CloseProgressDialog();
					AlertsService.ShowLongToast(this, "Wyst�pi� b��d podczas logowania z facebookiem");
				}
			};
			facebookLoginBtn.RegisterCallback(this.callbackManager, loginCallback);
		}

		private void SetUserNameInAppSettings(string userName)
		{
			SharedPreferencesHelper.SetUserNameInAppSettings(this, userName);
		}

		private void GoToActivity(Intent intentToStart)
		{
			this.Finish();
			StartActivity(intentToStart);
		}

		private async Task<TokenModel> LoginWithFacebook()
		{
			var fbTokenViewModel = new FacebookTokenViewModel { FacebookToken = AccessToken.CurrentAccessToken.Token };
			var bearerToken = await this.signInService.SignInUserWithFacebookToken(fbTokenViewModel);
			if (bearerToken != null)
			{
				preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, bearerToken.Token);
			}

			return bearerToken;
		}
	}

}