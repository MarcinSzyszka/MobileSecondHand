package marcin_szyszka.mobileseconndhand.services;

import android.support.v4.app.FragmentActivity;

import com.google.gson.Gson;
import com.loopj.android.http.JsonHttpResponseHandler;
import com.loopj.android.http.RequestParams;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.UnsupportedEncodingException;
import java.util.HashMap;
import java.util.Map;

import cz.msebera.android.httpclient.Header;
import cz.msebera.android.httpclient.HttpEntity;
import cz.msebera.android.httpclient.entity.ByteArrayEntity;
import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.EventListenerType;
import marcin_szyszka.mobileseconndhand.common.IAddNewAdvertisementItemPhotosRequestFinished;
import marcin_szyszka.mobileseconndhand.common.IAdvertisementItemsReceiver;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.CoordinatesModel;
import marcin_szyszka.mobileseconndhand.models.NewAdvertisementItem;

/**
 * Created by marcianno on 2016-03-07.
 */
public class AdvertisementItemsService {
    static IJsonObjectReceiveDelegate mDataReceiveObject;
    IAddNewAdvertisementItemPhotosRequestFinished mOnAddNewPhotosRequestFinishedListener;
    private static AdvertisementItemsService ourInstance = new AdvertisementItemsService();

    public static AdvertisementItemsService getInstance() {
        return ourInstance;
    }

    public void UploadNewAdvertisementPhotos(String filePath, IAddNewAdvertisementItemPhotosRequestFinished eventListener) {
        mOnAddNewPhotosRequestFinishedListener = eventListener;
        //docelowo kolekcja plików
        File myFile = new File(filePath);
        RequestParams params = new RequestParams();
        try {
            params.put("photo0", myFile);
        } catch (FileNotFoundException e) {
            raiseListenerCallback(500, null, EventListenerType.onAddNewAdvertisementItemPhotosRequestFinished);
        }
        HttpRequestsService.post("AdvertisementItem/UploadFiles", params, null, new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                raiseListenerCallback(statusCode, response, EventListenerType.onAddNewAdvertisementItemPhotosRequestFinished);
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                raiseListenerCallback(statusCode, null, EventListenerType.onAddNewAdvertisementItemPhotosRequestFinished);
            }

            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONArray response) {
                raiseListenerCallback(statusCode, null, EventListenerType.onAddNewAdvertisementItemPhotosRequestFinished);
            }

            @Override
            public void onSuccess(int statusCode, Header[] headers, String responseString) {
                raiseListenerCallback(statusCode, null, EventListenerType.onAddNewAdvertisementItemPhotosRequestFinished);
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                raiseListenerCallback(statusCode, errorResponse, EventListenerType.onAddNewAdvertisementItemPhotosRequestFinished);
            }
        });
    }

    public void CreateNewAdvertisementItem(NewAdvertisementItem newAdvertisementItem, FragmentActivity callingActivity, IJsonObjectReceiveDelegate dataReceiveObject) throws UnsupportedEncodingException {
        mDataReceiveObject = dataReceiveObject;
        String model = new Gson().toJson(newAdvertisementItem);

        HttpEntity entity = new ByteArrayEntity(model.getBytes("UTF-8"));
        HttpRequestsService.postWithData(callingActivity, "AdvertisementItem/CreateAdvertisementItem", entity, "application/json", new JsonHttpResponseHandler() {
            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONObject response) {
                raiseListenerCallback(statusCode, response, EventListenerType.onCreateNewAdvertisementItemRequestFinished);
            }


            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                raiseListenerCallback(statusCode, null, EventListenerType.onCreateNewAdvertisementItemRequestFinished);
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                raiseListenerCallback(statusCode, null, EventListenerType.onCreateNewAdvertisementItemRequestFinished);
            }
        });
    }

    public void GetAdvertisementItems(CoordinatesModel coordinatesModel, FragmentActivity callingActivity, final IAdvertisementItemsReceiver dataReceiveObject) throws UnsupportedEncodingException {
        String model = new Gson().toJson(coordinatesModel);
        HttpEntity entity = new ByteArrayEntity(model.getBytes("UTF-8"));

        String authenticationToken = SharedPreferencesService.getInstance().getSpecificSharedPreferenceString(callingActivity, R.string.authentication_token);
        Map<String, String> headers = new HashMap<>();
        headers.put("Authorization", "Bearer " + authenticationToken);

        HttpRequestsService.getWithData(callingActivity, "AdvertisementItem/GetAdvertisements", headers, entity, "application/json", new JsonHttpResponseHandler() {

            @Override
            public void onSuccess(int statusCode, Header[] headers, JSONArray response) {
                try {
                    dataReceiveObject.onAdvertisementItemsReceived(statusCode, response);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, Throwable throwable, JSONObject errorResponse) {
                try {
                    dataReceiveObject.onAdvertisementItemsReceived(statusCode, null);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onFailure(int statusCode, Header[] headers, String responseString, Throwable throwable) {
                try {
                    dataReceiveObject.onAdvertisementItemsReceived(statusCode, null);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }
        });
    }

    private void raiseListenerCallback(int statusCode, JSONObject response, EventListenerType eventListenerType) {
        switch (eventListenerType) {
            case onAddNewAdvertisementItemPhotosRequestFinished: {
                mOnAddNewPhotosRequestFinishedListener.onAddNewAdvertisementItemPhotosRequestFinished(statusCode, response);
                break;
            }
            case onCreateNewAdvertisementItemRequestFinished: {
                mDataReceiveObject.onDataReceived(statusCode, response);
                break;
            }
            case onGetAdvertisementsItemsRequestFinished: {
                mDataReceiveObject.onDataReceived(statusCode, response);
                break;
            }
        }
    }

    private void raiseListenerCallback(int statusCode, JSONArray response) {
        mDataReceiveObject.onDataReceived(statusCode, response);
    }

}
