package marcin_szyszka.mobileseconndhand.services;

import android.content.Context;

import com.facebook.AccessToken;
import com.loopj.android.http.AsyncHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import java.util.HashMap;
import java.util.Map;

import cz.msebera.android.httpclient.Header;
import marcin_szyszka.mobileseconndhand.IDataReceiveDelegate;
import marcin_szyszka.mobileseconndhand.R;

/**
 * Created by marcianno on 2016-02-12.
 */
public class SignInService {
    private static SignInService serviceInstance = new SignInService();
    IDataReceiveDelegate mDataReceiveObject;

    public static SignInService getInstance() {
        return serviceInstance;
    }

    public void userIsLogged(Context context, IDataReceiveDelegate dataReceiveObject) {
        mDataReceiveObject = dataReceiveObject;
        String authenticationToken = SharedPreferencesService.getInstance().getSpecificSharedPreferenceString(context, R.string.authentication_token);
        if (authenticationToken == null) {
            AccessToken facebookToken = AccessToken.getCurrentAccessToken();
            if (facebookToken != null) {
                //signInWithFacebook()
                //logowanko z fejsem
                raiseListenerCallback(200);
            } else {
                raiseListenerCallback(401);
            }
            return;
        }
        Map<String, String> headers = new HashMap<>();
        headers.put("Authorization", "Bearer " + authenticationToken);

        RequestParams req = new RequestParams();
        HttpRequestsService.get("WebApiAccount/TokenIsActual/", req, headers, new AsyncHttpResponseHandler() {

            @Override
            public void onSuccess(int statusCode, Header[] headers, byte[] responseBody) {
                raiseListenerCallback(statusCode);
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, byte[] responseBody, Throwable error) {
                raiseListenerCallback(statusCode);
            }
        });
    }

    private void raiseListenerCallback(int statusCode) {
        mDataReceiveObject.onDataReceived(statusCode);
    }

}
