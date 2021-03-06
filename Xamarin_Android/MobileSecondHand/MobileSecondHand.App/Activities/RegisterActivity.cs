using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Services.Authentication;

namespace MobileSecondHand.App.Activities
{
	[Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class RegisterActivity : AppCompatActivity
	{
		private EditText confirmPasswordInput;
		private EditText emailInput;
		private EditText passwordInput;
		private Button submitRegistrationBtn;
		private View focusView;
		private ISignInService signInService;
		private ProgressDialogHelper progress;
		private CheckBox acceptCheckboxLogin;
		private TextView textViewAcceptReg;

		public RegisterActivity()
		{
			this.signInService = new SignInService();
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.RegisterActivity);
			SetupViews();
		}

		private void SetupViews()
		{
			this.progress = new ProgressDialogHelper(this);
			emailInput = FindViewById<EditText>(Resource.Id.inputEmailRegistration);
			passwordInput = FindViewById<EditText>(Resource.Id.inputPasswordRegistration);
			confirmPasswordInput = FindViewById<EditText>(Resource.Id.inputConfirmPasswordRegistration);
			submitRegistrationBtn = FindViewById<Button>(Resource.Id.buttonSubmitRegistration);
			submitRegistrationBtn.Click += async (s, e) =>
			{
				submitRegistrationBtn.Enabled = false;
				await SubmitRegistrationBtn_Click(s, e);
				submitRegistrationBtn.Enabled = true;
			};
			SetupAcceptCheckbox();
		}

		private void SetupAcceptCheckbox()
		{
			acceptCheckboxLogin = FindViewById<CheckBox>(Resource.Id.acceptCheckboxRegister);
			textViewAcceptReg = FindViewById<TextView>(Resource.Id.textViewAcceptReg);
			textViewAcceptReg.TextFormatted = Html.FromHtml("Akceptuj� <a href='" + WebApiConsts.WEB_API_URL + "file/reg'>Regulamin</a> oraz <a href='" + WebApiConsts.WEB_API_URL + "file/privpolicy'>Polityk� Prywatno�ci</a>.");
			textViewAcceptReg.MovementMethod = LinkMovementMethod.Instance;
		}

		private async Task SubmitRegistrationBtn_Click(object sender, EventArgs e)
		{
			if (loginFormIsValid())
			{
				if (!acceptCheckboxLogin.Checked)
				{
					AlertsService.ShowShortToast(this, "Zapoznaj si� i zaakceptuj Regulamin oraz Polityk� Prywatno�ci");
				}
				else
				{
					progress.ShowProgressDialog("Trwa tworzenie konta u�ytkownika... Prosz� czeka�");
					await RegisterUser();
					progress.CloseProgressDialog();
				}
			}
			else
			{
				focusView.RequestFocus();
			}
		}

		private bool loginFormIsValid()
		{
			var formValidator = new FormValidator();
			return formValidator.IsLoginFormValidate(emailInput, passwordInput, confirmPasswordInput, ref focusView);
		}

		private async Task RegisterUser()
		{
			var registerModel = new RegisterModel
			{
				Email = emailInput.Text,
				Password = passwordInput.Text,
				ConfirmPassword = confirmPasswordInput.Text
			};

			var tokenModel = await this.signInService.RegisterUser(registerModel);
			if (tokenModel != null)
			{
				if (String.IsNullOrEmpty(tokenModel.Token))
				{
					AlertsService.ShowLongToast(this, "Podany email ju� istnieje w bazie u�ytkownik�w!");
				}
				else
				{
					var preferenceHelper = new SharedPreferencesHelper(this);
					preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, tokenModel.Token);
					GoToStartActivity();
				}
			}
			else
			{
				AlertsService.ShowLongToast(this, "Co� posz�o nie tak na serwerze!");
			}

		}
		private void GoToStartActivity()
		{
			var startIntent = new Intent(this, typeof(StartActivity));
			this.Finish();
			StartActivity(startIntent);
		}
	}
}