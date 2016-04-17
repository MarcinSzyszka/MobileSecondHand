using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MobileSecondHand.App {
	[Activity(Label = "RegisterActivity")]
	public class RegisterActivity : Activity {
		private EditText confirmPasswordInput;
		private EditText emailInput;
		private EditText passwordInput;
		private Button submitRegistrationBtn;
		private View focusView;
		private ISignInService signInService;

		public RegisterActivity() {
			this.signInService = new SignInService();
		}

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.RegisterActivity);
			SetupViews();
		}

		private void SetupViews() {
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
			var registerModel = new RegisterModel {
				Email = emailInput.Text,
				Password = passwordInput.Text,
				ConfirmPassword = confirmPasswordInput.Text
			};

			var tokenModel = await this.signInService.RegisterUser(registerModel);
			if (tokenModel != null) {
				var preferenceHelper = new SharedPreferencesHelper(this);
				preferenceHelper.SetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN, tokenModel.Token);
				GoToMainActivity();
			}
			else {
				AlertsService.ShowToast(this, "Coœ posz³o nie tak na serwerze!");
			}
		}
		private void GoToMainActivity() {
			var mainIntent = new Intent(this, typeof(MainActivity));
			this.Finish();
			StartActivity(mainIntent);
		}
	}
}