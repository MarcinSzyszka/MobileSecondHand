package marcin_szyszka.mobileseconndhand.common;

import org.json.JSONArray;
import org.json.JSONObject;

/**
 * Created by marcianno on 2016-02-14.
 */
public interface IDataReceiveDelegate {

    void onDataReceived(int statusCode, JSONArray response);
    void onDataReceived(int statusCode);
}
