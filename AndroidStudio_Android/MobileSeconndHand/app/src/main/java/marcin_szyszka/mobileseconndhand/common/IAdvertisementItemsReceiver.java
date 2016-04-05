package marcin_szyszka.mobileseconndhand.common;

import org.json.JSONArray;
import org.json.JSONException;

/**
 * Created by marcianno on 2016-03-14.
 */
public interface IAdvertisementItemsReceiver {
    void onAdvertisementItemsReceived(int status, JSONArray response) throws JSONException;
}
