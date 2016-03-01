package marcin_szyszka.mobileseconndhand.services;

import android.content.Context;

import com.facebook.AccessToken;
import com.google.gson.Gson;
import com.loopj.android.http.AsyncHttpResponseHandler;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.InputStreamReader;
import java.util.HashMap;
import java.util.Map;

import cz.msebera.android.httpclient.Header;
import marcin_szyszka.mobileseconndhand.common.IDataReceiveDelegate;
import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.TokenModel;

/**
 * Created by marcianno on 2016-02-12.
 */
public class SignInService {
    private static SignInService serviceInstance = new SignInService();
    IJsonObjectReceiveDelegate mDataReceiveObject;

    public static SignInService getInstance() {
        return serviceInstance;
    }

    public void checkIsUserLogged(Context context, IJsonObjectReceiveDelegate dataReceiveObject) {
        mDataReceiveObject = dataReceiveObject;


        //w developmencie
        raiseListenerCallback(401, new JSONObject());


        String authenticationToken = SharedPreferencesService.getInstance().getSpecificSharedPreferenceString(context, R.string.authentication_token);
        if (authenticationToken == null) {
            AccessToken facebookToken = AccessToken.getCurrentAccessToken();
            if (facebookToken != null) {
                signInWithFacebook(facebookToken.getToken(), dataReceiveObject);
            } else {
                raiseListenerCallback(401, new JSONObject());
            }
        }
        else{
            isTokenValid(authenticationToken);
        }
    }

    private void isTokenValid(String authenticationToken) {
        Map<String, String> headers = new HashMap<>();
        headers.put("Authorization", "Bearer " + authenticationToken);

        RequestParams req = new RequestParams();
        HttpRequestsService.get("WebApiAccount/TokenIsActual/", req, headers, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                raiseListenerCallback(statusCode, response);
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                raiseListenerCallback(statusCode, errorResponse);
            }
        });
    }

    public void signInWithFacebook(String token, IJsonObjectReceiveDelegate dataReceiveObject) {
        mDataReceiveObject = dataReceiveObject;
        RequestParams params = new RequestParams();
        params.put("facebookToken", token);

        HttpRequestsService.post("WebApiAccount/LoginWithFacebook/", params, null, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                raiseListenerCallback(statusCode, response);
            }


            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                raiseListenerCallback(statusCode, errorResponse);
            }

        });
    }


    private void raiseListenerCallback(int statusCode, JSONObject response) {
        mDataReceiveObject.onDataReceived(statusCode, response);
    }

}