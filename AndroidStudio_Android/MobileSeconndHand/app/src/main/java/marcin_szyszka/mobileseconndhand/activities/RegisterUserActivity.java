package marcin_szyszka.mobileseconndhand.activities;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.google.gson.Gson;

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.RegisterUserModel;
import marcin_szyszka.mobileseconndhand.models.TokenModel;
import marcin_szyszka.mobileseconndhand.services.SignInService;

public class RegisterUserActivity extends AppCompatActivity implements IJsonObjectReceiveDelegate {

    private EditText mEmailTextView;
    private EditText mPasswordTextView;
    private EditText mConfirmPasswordTextView;
    private Button mRegisterButton;
    private View mProgressBar;
    private View focusView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register_user);
        mProgressBar = findViewById(R.id.progressBar);
        mProgressBar.setVisibility(View.INVISIBLE);

        mEmailTextView = (EditText) findViewById(R.id.registerInputEmail);
        mPasswordTextView = (EditText) findViewById(R.id.registerInputPassword);
        mConfirmPasswordTextView = (EditText) findViewById(R.id.registerInputConfirmPassword);
        mRegisterButton = (Button) findViewById(R.id.register_button);

        mRegisterButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                reigisterUser();
            }
        });
    }

    private void reigisterUser() {
        // Store values at the time of the login attempt.
        boolean formIsValid = validateRegisterForm();

        if (!formIsValid) {
            // There was an error; don't attempt login and focus the first
            // form field with an error.
            focusView.requestFocus();
        } else {
            // Show a progress spinner, and kick off a background task to
            // perform the user login attempt.
            mProgressBar.setVisibility(View.VISIBLE);
            RegisterUserModel registerModel = new RegisterUserModel();
            registerModel.Email = mEmailTextView.getText().toString();
            registerModel.Password = mPasswordTextView.getText().toString();
            registerModel.ConfirmPassword = mConfirmPasswordTextView.getText().toString();

            try {
                SignInService.getInstance().registerUser(this, registerModel, getBaseContext(), this);
            } catch (Exception exc) {
                String message = exc.getMessage();
            }

        }
    }

    private boolean validateRegisterForm() {
        String email = mEmailTextView.getText().toString();
        String password = mPasswordTextView.getText().toString();
        String confirmedPassword = mConfirmPasswordTextView.getText().toString();

        if (TextUtils.isEmpty(email)) {
            mEmailTextView.setError(getString(R.string.error_field_required));
            focusView = mEmailTextView;
            return false;
        } else if (!isEmailValid(email)) {
            mEmailTextView.setError(getString(R.string.error_invalid_email));
            focusView = mEmailTextView;
            return false;
        } else if (TextUtils.isEmpty(password)) {
            mPasswordTextView.setError(getString(R.string.error_field_required));
            focusView = mPasswordTextView;
            return false;
        } else if (!isPasswordValid(password)) {
            mPasswordTextView.setError(getString(R.string.error_invalid_password));
            focusView = mPasswordTextView;
            return false;
        } else if (TextUtils.isEmpty(confirmedPassword)) {
            mConfirmPasswordTextView.setError(getString(R.string.error_field_required));
            focusView = mConfirmPasswordTextView;
            return false;
        } else if (!isConfirmedPasswordValid(confirmedPassword, password)) {
            mConfirmPasswordTextView.setError(getString(R.string.error_passwords_are_incorrect));
            focusView = mConfirmPasswordTextView;
            return false;
        }
        else{
            return true;
        }
    }

    private boolean isConfirmedPasswordValid(String confirmedPassword, String password) {
        if (confirmedPassword.contentEquals(password)) {
            return true;
        }
        return false;
    }

    private boolean isEmailValid(String email) {
        Matcher matcher = Pattern.compile("^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,6}$", Pattern.CASE_INSENSITIVE).matcher(email);
        return matcher.find();
    }

    private boolean isPasswordValid(String password) {
        if (password.length() < 6) return false;
        Matcher bigCaseMatcher = Pattern.compile("[A-Z]+").matcher(password);
        Matcher digitMatcher = Pattern.compile("\\d+").matcher(password);
        Matcher nonLetterOrDigitMatcher = Pattern.compile("\\W+").matcher(password);
        if (!bigCaseMatcher.find()) return false;
        if (!digitMatcher.find()) return false;
        if (!nonLetterOrDigitMatcher.find()) return false;

        return true;
    }


    public void onDataReceived(int statusCode, JSONObject response) {
        mProgressBar.setVisibility(View.INVISIBLE);
        if (statusCode == 200) {
            TokenModel tokenModel = new Gson().fromJson(response.toString(), TokenModel.class);

            SharedPreferences preferences = this.getBaseContext().getSharedPreferences(getString(R.string.app_shared_preferences), Context.MODE_PRIVATE);
            SharedPreferences.Editor editor  = preferences.edit();
            editor.putString(getString(R.string.authentication_token), tokenModel.Token);
            editor.commit();

            Toast.makeText(this, "Token jest ok", Toast.LENGTH_LONG).show();
            this.finish();
            Intent mainActivity = new Intent(this, MainActivity.class);
            startActivity(mainActivity);
        } else {
            Toast.makeText(this, "Coś poszło nie tak. Nie mogę połączyć się z serwerem", Toast.LENGTH_LONG).show();
            this.finish();
        }
    }

    @Override
    public void onDataReceived(int statusCode, JSONArray response) {

    }
}
