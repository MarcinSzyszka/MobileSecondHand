package marcin_szyszka.mobileseconndhand.services;

import android.content.Context;
import android.content.SharedPreferences;

import marcin_szyszka.mobileseconndhand.R;

/**
 * Created by marcianno on 2016-02-12.
 */
public class SharedPreferencesService {
    private static SharedPreferencesService ourInstance = new SharedPreferencesService();

    public static SharedPreferencesService getInstance(){
        return ourInstance;
    }

    public String getSpecificSharedPreferenceString(Context context, int preferenceId) {
        SharedPreferences preferences = context.getSharedPreferences(context.getString(R.string.app_shared_preferences), Context.MODE_PRIVATE);
        return preferences.getString(context.getString(preferenceId), null);
    }
}
