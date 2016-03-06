package marcin_szyszka.mobileseconndhand.activities;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Configuration;
import android.support.v7.app.AppCompatActivity;

import android.os.AsyncTask;

import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.facebook.AccessToken;
import com.facebook.CallbackManager;
import com.facebook.FacebookCallback;
import com.facebook.FacebookException;
import com.facebook.login.LoginResult;
import com.facebook.login.widget.LoginButton;
import com.google.gson.Gson;

import org.json.JSONObject;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.ErrorResponse;
import marcin_szyszka.mobileseconndhand.models.LoginModel;
import marcin_szyszka.mobileseconndhand.models.TokenModel;
import marcin_szyszka.mobileseconndhand.services.SignInService;

/**
 * A login screen that offers login via email/password.
 */
public class LoginActivity extends AppCompatActivity implements IJsonObjectReceiveDelegate {

    // UI references.
    private EditText mEmailTextView;
    private EditText mPasswordTextView;
    private View mProgressBar;
    private View focusView;
    private CallbackManager callbackManager;
    private AccessToken accessToken;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        callbackManager = CallbackManager.Factory.create();
        setContentView(R.layout.activity_login);

        mEmailTextView = (EditText) findViewById(R.id.inputEmail);
        mPasswordTextView = (EditText) findViewById(R.id.inputPassword);
        mProgressBar = findViewById(R.id.progressBar);

        mProgressBar.setVisibility(View.INVISIBLE);

        Button mEmailSignInButton = (Button) findViewById(R.id.loginFormSubmit);
        mEmailSignInButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                attemptLogin();
            }
        });

        Button mLoginRegisterButton = (Button) findViewById(R.id.login_register_button);
        mLoginRegisterButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                goToRegister();
            }
        });

        LoginButton loginButton = (LoginButton) findViewById(R.id.login_facebook_button);
        loginButton.setReadPermissions("public_profile", "email");

//        LoginManager.getInstance().logOut();
        loginButton.registerCallback(callbackManager, new FacebookCallback<LoginResult>() {
            @Override
            public void onSuccess(LoginResult loginResult) {
                accessToken = loginResult.getAccessToken();
                if (accessToken != null) {
                    ShowMessage("OK");
                    loginWithFacebook();
                } else {
                    ShowMessage("Token pusty");
                }

            }

            @Override
            public void onCancel() {
                ShowMessage("Przerwano");
            }

            @Override
            public void onError(FacebookException exception) {
                ShowMessage("Błąd: " + exception.getMessage());
            }
        });
    }

    private void loginWithFacebook() {
        SignInService.getInstance().checkIsUserLogged(getBaseContext(), this);
    }

    private void goToRegister() {
        Intent registerActivity = new Intent(this, RegisterUserActivity.class);
        startActivity(registerActivity);
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        setContentView(R.layout.activity_login);
        TextView text = (TextView) findViewById(R.id.otherFacebookInfo);
        if (newConfig.screenHeightDp > 350) {
            text.setVisibility(View.VISIBLE);
        } else {
            text.setVisibility(View.INVISIBLE);
        }
    }


    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        callbackManager.onActivityResult(requestCode, resultCode, data);
    }

    private void ShowMessage(String message) {
        Toast.makeText(this, message, Toast.LENGTH_LONG).show();
    }

    /**
     * Attempts to sign in or register the account specified by the login form.
     * If there are form errors (invalid email, missing fields, etc.), the
     * errors are presented and no actual login attempt is made.
     */
    private void attemptLogin() {
        boolean loginFormIsValid = validateRegisterForm();

        if (!loginFormIsValid) {
            focusView.requestFocus();
        } else {
            mProgressBar.setVisibility(View.VISIBLE);
            String email = mEmailTextView.getText().toString();
            String password = mPasswordTextView.getText().toString();
            LoginModel loginModel = new LoginModel();
            loginModel.Email = email;
            loginModel.Password = password;

            try {
                SignInService.getInstance().loginUser(this, loginModel, getBaseContext(), this);
            } catch (Exception exc) {
                String message = exc.getMessage();
            }
        }
    }

    private boolean validateRegisterForm() {
        String email = mEmailTextView.getText().toString();
        String password = mPasswordTextView.getText().toString();

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
        } else {
            return true;
        }
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

    @Override
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
            if (response != null) {
                ErrorResponse errorResponse = new Gson().fromJson(response.toString(), ErrorResponse.class);
                Toast.makeText(this, errorResponse.ErrorMessage, Toast.LENGTH_LONG).show();
            }
            else{
                Toast.makeText(this, "Wystąił nieoczekiwany błąd", Toast.LENGTH_LONG).show();
            }
        }
    }

}

