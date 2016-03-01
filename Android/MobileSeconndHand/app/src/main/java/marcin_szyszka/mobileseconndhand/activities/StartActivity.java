package marcin_szyszka.mobileseconndhand.activities;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import com.facebook.FacebookSdk;
import com.google.gson.Gson;

import org.json.JSONObject;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.TokenModel;
import marcin_szyszka.mobileseconndhand.services.NetworkOperationsService;
import marcin_szyszka.mobileseconndhand.services.SignInService;

import static android.app.PendingIntent.getActivity;

public class StartActivity extends AppCompatActivity implements IJsonObjectReceiveDelegate {


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

        //wpisuje zeby bylo null dla developmentu
        SharedPreferences preferences = this.getBaseContext().getSharedPreferences(getString(R.string.app_shared_preferences), Context.MODE_PRIVATE);
        SharedPreferences.Editor editor  = preferences.edit();
        editor.putString(getString(R.string.authentication_token), null);
        editor.commit();

        SignInService.getInstance().checkIsUserLogged(getBaseContext(), this);
    }


    @Override
    protected void onResume() {
        super.onResume();
    }

    @Override
    public void onDataReceived(int statusCode, JSONObject response) {
        if (statusCode == 401) {
            Toast.makeText(this, "Musisz się zalogować", Toast.LENGTH_LONG).show();
            this.finish();
            Intent intent = new Intent(this, LoginActivity.class);
            startActivity(intent);
        } else if (statusCode == 200) {
            TokenModel tokenModel = new Gson().fromJson(response.toString(), TokenModel.class);

            //zapis tokenu
            /*SharedPreferences preferences = this.getBaseContext().getSharedPreferences(getString(R.string.app_shared_preferences), Context.MODE_PRIVATE);
            SharedPreferences.Editor editor  = preferences.edit();
            editor.putString(getString(R.string.authentication_token), tokenModel.Token);
            editor.commit();*/

            Toast.makeText(this, "Token jest ok", Toast.LENGTH_LONG).show();
            // startMainActivity();
            this.finish();
            Intent mainActivity = new Intent(this, MainActivity.class);
            startActivity(mainActivity);
        }
        else{
            Toast.makeText(this, "Coś poszło nie tak. Nastąpi zamknięcie aplikacji", Toast.LENGTH_LONG).show();
            this.finish();
        }
    }
}
