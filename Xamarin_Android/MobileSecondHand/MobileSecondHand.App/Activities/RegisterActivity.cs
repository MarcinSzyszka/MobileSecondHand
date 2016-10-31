using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Services.Authentication;

namespace MobileSecondHand.App.Activities
{
	[Activity]
	public class RegisterActivity : AppCompatActivity {
		private EditText confirmPasswordInput;
		private EditText emailInput;
		private EditText passwordInput;
		private Button submitRegistrationBtn;
		private View focusView;
		private ISignInService signInService;
		private ProgressDialogHelper progress;

		public RegisterActivity() {
			this.signInService = new SignInService();
		}

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.RegisterActivity);
			SetupViews();
		}

		private void SetupViews() {
			this.progress = new ProgressDialogHelper(this);
			emailInput = FindViewById<EditText>(Resource.Id.inputEmailRegistration);
			passwordInput = FindViewById<EditText>(Resource.Id.inputPasswordRegistration);
			confirmPasswordInput = FindViewById<EditText>(Resource.Id.inputConfirmPasswordRegistration);
			submitRegistrationBtn = FindViewById<Button>(Resource.Id.buttonSubmitRegistration);

			submitRegistrationBtn.Click += async (s, e) => await  SubmitRegistrationBtn_Click(s, e);
		}

		private async Task SubmitRegistrationBtn_Click(object sender, EventArgs e) {
			if (loginFormIsValid()) {
				await RegisterUser();
			}
			else {
				focusView.RequestFocus();
			}
		}

		private bool loginFormIsValid() {
			var formValidator = new FormValidator();
			return formValidator.IsLoginFormValidate(emailInput, passwordInput, confirmPasswordInput, ref focusView);
		}

		private async Task RegisterUser() {
			progress.ShowProgressDialog("Trwa tworzenie konta u¿ytkownika... Proszê czekaæ");
			var registerModel = new RegisterModel {
				Email = emailInput.Text,
				Password = passwordInput.Text,
				ConfirmPassword = confirmPasswordInput.Text
			};

			var tokenModel = await this.signInService.RegisterUser(registerModel);
			if (tokenModel != null) {
				var preferenceHelper = new SharedPreferencesHelper(this);
				preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, tokenModel.Token);
				progress.CloseProgressDialog();
				GoToStartActivity();
			}
			else {
				progress.CloseProgressDialog();
				AlertsService.ShowLongToast(this, "Coœ posz³o nie tak na serwerze!");
			}
			
		}
		private void GoToStartActivity() {
			var startIntent = new Intent(this, typeof(StartActivity));
			this.Finish();
			StartActivity(startIntent);
		}
	}
}