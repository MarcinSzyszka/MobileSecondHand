package marcin_szyszka.mobileseconndhand.services;

import android.content.Context;
import android.support.v7.app.AppCompatActivity;

import com.facebook.AccessToken;
import com.google.gson.Gson;
import com.loopj.android.http.AsyncHttpResponseHandler;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import java.util.HashMap;
import java.util.Map;

import cz.msebera.android.httpclient.Header;
import cz.msebera.android.httpclient.HttpEntity;
import cz.msebera.android.httpclient.entity.ByteArrayEntity;
import marcin_szyszka.mobileseconndhand.activities.LoginActivity;
import marcin_szyszka.mobileseconndhand.activities.RegisterUserActivity;
import marcin_szyszka.mobileseconndhand.common.IDataReceiveDelegate;
import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.LoginModel;
import marcin_szyszka.mobileseconndhand.models.RegisterUserModel;
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

        String authenticationToken = SharedPreferencesService.getInstance().getSpecificSharedPreferenceString(context, R.string.authentication_token);
        if (authenticationToken == null) {
            tryWithFacebook();
        } else {
            isTokenValid(authenticationToken);
        }
    }

    private void tryWithFacebook() {
        AccessToken facebookToken = AccessToken.getCurrentAccessToken();
        if (facebookToken != null) {
            signInWithFacebook(facebookToken.getToken());
        } else {
            raiseListenerCallback(401, new JSONObject());
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
            public void onSuccess(int statusCode, Header[] headers, String responseString) {
                if (statusCode == 401) {
                    tryWithFacebook();
                } else {
                    raiseListenerCallback(statusCode, null);
                }
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                if (statusCode == 401) {
                    tryWithFacebook();
                } else {
                    raiseListenerCallback(statusCode, null);
                }
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                if (statusCode == 401) {
                    tryWithFacebook();
                } else {
                    raiseListenerCallback(statusCode, null);
                }
            }
        });
    }

    public void signInWithFacebook(String token) {
        RequestParams params = new RequestParams();
        params.put("facebookToken", token);

        HttpRequestsService.post("WebApiAccount/LoginWithFacebook/", params, null, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                raiseListenerCallback(statusCode, response);
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                raiseListenerCallback(statusCode, null);
            }


            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                raiseListenerCallback(statusCode, errorResponse);
            }

        });
    }

    public void registerUser(AppCompatActivity userActivity, RegisterUserModel registerModel, Context baseContext, IJsonObjectReceiveDelegate dataReceiveObject) throws UnsupportedEncodingException {
        mDataReceiveObject = dataReceiveObject;
        String model = new Gson().toJson(registerModel);
        HttpEntity entity = new ByteArrayEntity(model.getBytes("UTF-8"));
        HttpRequestsService.postWithData(userActivity, "WebApiAccount/Register", entity, "application/json", new JsonHttpResponseHandler() {
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

    public void loginUser(AppCompatActivity userActivity, LoginModel loginModel, Context baseContext, IJsonObjectReceiveDelegate dataReceiveObject) throws UnsupportedEncodingException {
        mDataReceiveObject = dataReceiveObject;
        String model = new Gson().toJson(loginModel);
        HttpEntity entity = new ByteArrayEntity(model.getBytes("UTF-8"));
        HttpRequestsService.postWithData(userActivity, "WebApiAccount/LoginStandard", entity, "application/json", new JsonHttpResponseHandler() {
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