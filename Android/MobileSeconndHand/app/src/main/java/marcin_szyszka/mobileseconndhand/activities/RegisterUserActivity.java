package marcin_szyszka.mobileseconndhand.activities;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import marcin_szyszka.mobileseconndhand.R;

public class RegisterUserActivity extends AppCompatActivity {

    private EditText mEmailTextView;
    private EditText mPasswordTextView;
    private EditText mConfirmPasswordTextView;
    private Button mRegisterButton;
    private View mProgressBar;

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
        String email = mEmailTextView.getText().toString();
        String password = mPasswordTextView.getText().toString();
        String confirmedPassword = mConfirmPasswordTextView.getText().toString();

        boolean cancel = false;
        View focusView = null;

        // Check for a valid password, if the user entered one.
        if (TextUtils.isEmpty(password) || !isPasswordValid(password)) {
            mPasswordTextView.setError(getString(R.string.error_invalid_password));
            focusView = mPasswordTextView;
            cancel = true;
        }

        // Check for a valid confirmed password, if the user entered one.
        if (TextUtils.isEmpty(confirmedPassword) || !isConfirmedPasswordValid(confirmedPassword, password)) {
            mPasswordTextView.setError(getString(R.string.error_passwords_are_incorrect));
            focusView = mPasswordTextView;
            cancel = true;
        }

        // Check for a valid email address.
        if (TextUtils.isEmpty(email)) {
            mEmailTextView.setError(getString(R.string.error_field_required));
            focusView = mEmailTextView;
            cancel = true;
        } else if (!isEmailValid(email)) {
            mEmailTextView.setError(getString(R.string.error_invalid_email));
            cancel = true;
        }

        if (cancel) {
            // There was an error; don't attempt login and focus the first
            // form field with an error.
            focusView.requestFocus();
        } else {
            // Show a progress spinner, and kick off a background task to
            // perform the user login attempt.
            //logowanko
            mProgressBar = findViewById(R.id.progressBar);
            mProgressBar.setVisibility(View.VISIBLE);
        }
    }

    private boolean isConfirmedPasswordValid(String confirmedPassword, String password) {
        if (confirmedPassword.equalsIgnoreCase(password)){
            return  true;
        }
        return false;
    }

    private boolean isEmailValid(String email) {
        //TODO: Replace this with your own logic
        return email.contains("@");
    }

    private boolean isPasswordValid(String password) {
        //TODO: Replace this with your own logic
        return password.length() > 6;
    }

}
