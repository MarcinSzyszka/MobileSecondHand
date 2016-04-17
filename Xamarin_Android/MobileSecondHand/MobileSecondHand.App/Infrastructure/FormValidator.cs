using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileSecondHand.App.Infrastructure {
	public class FormValidator {
		string requiredFieldErrorMessage = "To pole jest wymagane";
		string incorrectEmailErrorMessage = "Podane email jest b³êdny";
		string incorrectPasswordErrorMessage = "Min. 6 znaków w tym conajmniej jedna cyfra";
		string confirmedPasswordErrorMessage = "Podane has³a nie s¹ takie same";
		public bool IsLoginFormValidate(EditText email, EditText password, ref View focus) {
			var icon = email.Context.GetDrawable(Android.Resource.Drawable.StatNotifyError);
			var emailText = email.Text;
			var passwordText = password.Text;
			if (emailText == null || emailText == String.Empty) {
				email.SetError(requiredFieldErrorMessage, icon);
				focus = email;
				return false;
			}
			Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
			if (!emailRegex.IsMatch(emailText)) {
				email.SetError(incorrectEmailErrorMessage, icon);
				focus = email;
				return false;
			}
			else if (passwordText == null || passwordText == String.Empty) {
				password.SetError(requiredFieldErrorMessage, icon);
				focus = password;
				return false;
			}
			Regex passwordRegex = new Regex(@"\d+");
			if (passwordText.Length < 6 || (passwordText.Length > 6 && !passwordRegex.IsMatch(passwordText))) {
				password.SetError(incorrectPasswordErrorMessage, icon);
				focus = password;
				return false;
			}

			return true;
		}

		public bool IsLoginFormValidate(EditText email, EditText password, EditText confirmedPassword, ref View focus) {
			var icon = email.Context.GetDrawable(Android.Resource.Drawable.StatNotifyError);
			var isValidate = false;
			isValidate = IsLoginFormValidate(email, password, ref focus);
			var passwordText = password.Text;
			var confirmedPasswordText = confirmedPassword.Text;
			if (isValidate) {
				if (passwordText.ToLower() != confirmedPasswordText.ToLower()) {
					confirmedPassword.SetError(confirmedPasswordErrorMessage, icon);
					focus = confirmedPassword;
					isValidate = false;
				}
			}

			return isValidate;
		}
	}
}