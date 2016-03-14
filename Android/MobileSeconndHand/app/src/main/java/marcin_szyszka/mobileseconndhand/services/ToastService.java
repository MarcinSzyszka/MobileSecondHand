package marcin_szyszka.mobileseconndhand.services;

import android.content.Context;
import android.widget.Toast;

/**
 * Created by marcianno on 2016-03-14.
 */
public class ToastService {
    private static ToastService ourInstance = new ToastService();

    public static ToastService getInstance() {
        return ourInstance;
    }

    public void showToast(Context context, String message){
        Toast.makeText(context, message, Toast.LENGTH_LONG).show();
    }
}
