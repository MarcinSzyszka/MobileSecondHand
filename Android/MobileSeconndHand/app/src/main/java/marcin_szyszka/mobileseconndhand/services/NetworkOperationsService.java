package marcin_szyszka.mobileseconndhand.services;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;

/**
 * Created by marcianno on 2016-02-12.
 */
public class NetworkOperationsService {
    private static NetworkOperationsService ourInstance = new NetworkOperationsService();

    public static NetworkOperationsService getInstance() {
        return ourInstance;
    }

    public boolean userIsConnectedToInternet(Context baseContext){
        ConnectivityManager connMgr = (ConnectivityManager) baseContext.getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connMgr.getActiveNetworkInfo();
        if (networkInfo != null && networkInfo.isConnected()) {
            return true;
        } else {
            return false;
        }
    }


}
