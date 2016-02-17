package marcin_szyszka.mobileseconndhand;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.os.Handler;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import com.facebook.FacebookSdk;

import marcin_szyszka.mobileseconndhand.services.NetworkOperationsService;
import marcin_szyszka.mobileseconndhand.services.SignInService;

import static android.app.PendingIntent.getActivity;

public class StartActivity extends AppCompatActivity implements IDataReceiveDelegate {


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        FacebookSdk.sdkInitialize(this.getApplicationContext());
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_start);
        View contentView = findViewById(R.id.fullscreen_content);
        contentView.setSystemUiVisibility(View.SYSTEM_UI_FLAG_LOW_PROFILE
                | View.SYSTEM_UI_FLAG_FULLSCREEN
                | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION);
        if(!NetworkOperationsService.getInstance().userIsConnectedToInternet(getBaseContext())){
            //not network
            Toast.makeText(this, "Brak internetu", Toast.LENGTH_LONG).show();
        }
        SharedPreferences preferences = this.getBaseContext().getSharedPreferences(getString(R.string.app_shared_preferences), Context.MODE_PRIVATE);
        SharedPreferences.Editor editor  = preferences.edit();
        editor.putString(getString(R.string.authentication_token), null);
        editor.commit();
        SignInService.getInstance().userIsLogged(getBaseContext(), this);

    }

    private void startMainActivity() {
        Intent mainActivity = new Intent(this, MainActivity.class);
        startActivity(mainActivity);
    }


    @Override
    protected void onResume() {
        super.onResume();
    }


    @Override
    public void onDataReceived(int statusCode) {
        if (statusCode != 200){
            Toast.makeText(this, "Trza sie zalogować", Toast.LENGTH_LONG).show();
            this.finish();
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
        }
        else{
            Toast.makeText(this, "Token jest ok", Toast.LENGTH_LONG).show();
           // startMainActivity();
            this.finish();
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
        }
    }
}
