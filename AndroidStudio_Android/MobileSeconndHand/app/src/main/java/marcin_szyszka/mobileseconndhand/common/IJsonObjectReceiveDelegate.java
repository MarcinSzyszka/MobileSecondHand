package marcin_szyszka.mobileseconndhand.common;

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Created by marcianno on 2016-03-01.
 */
public interface IJsonObjectReceiveDelegate {
    void onDataReceived(int statusCode, JSONObject response);
    void onDataReceived(int statusCode, JSONArray response);
}
