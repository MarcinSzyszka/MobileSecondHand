package marcin_szyszka.mobileseconndhand.activities;

import android.app.ProgressDialog;
import android.content.Context;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.GridLayoutManager;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.IAdvertisementItemsReceiver;
import marcin_szyszka.mobileseconndhand.common.IJsonObjectReceiveDelegate;
import marcin_szyszka.mobileseconndhand.models.AdvertisementItemShortModel;
import marcin_szyszka.mobileseconndhand.models.CoordinatesModel;
import marcin_szyszka.mobileseconndhand.services.AdvertisementItemsService;
import marcin_szyszka.mobileseconndhand.services.GpsLocationService;
import marcin_szyszka.mobileseconndhand.services.ToastService;

import java.io.UnsupportedEncodingException;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

/**
 * A fragment representing a list of Items.
 * <p/>
 * Activities containing this fragment MUST implement the {@link OnListFragmentInteractionListener}
 * interface.
 */
public class AdvertisementItemFragment extends Fragment implements IAdvertisementItemsReceiver {

    // TODO: Customize parameter argument names
    private static final String ARG_COLUMN_COUNT = "column-count";
    // TODO: Customize parameters
    private int mColumnCount = 1;
    private OnListFragmentInteractionListener mListener;
    private GpsLocationService gps;
    RecyclerView recyclerView;
    private MainActivity mMainActivity;

    /**
     * Mandatory empty constructor for the fragment manager to instantiate the
     * fragment (e.g. upon screen orientation changes).
     */
    public AdvertisementItemFragment() {
    }

    // TODO: Customize parameter initialization
    @SuppressWarnings("unused")
    public static AdvertisementItemFragment newInstance(int columnCount) {
        AdvertisementItemFragment fragment = new AdvertisementItemFragment();
        Bundle args = new Bundle();
        args.putInt(ARG_COLUMN_COUNT, columnCount);
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            mColumnCount = getArguments().getInt(ARG_COLUMN_COUNT);
        }
        gps = new GpsLocationService(this.getContext());
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_item_list, container, false);

        if (!gps.canGetLocation()) {
            gps.showSettingsAlert();
        } else {
            // Set the adapter
            if (view instanceof RecyclerView) {
                Context context = view.getContext();
                recyclerView = (RecyclerView) view;
                if (mColumnCount <= 1) {
                    recyclerView.setLayoutManager(new LinearLayoutManager(context));
                } else {
                    recyclerView.setLayoutManager(new GridLayoutManager(context, mColumnCount));
                }

                try {
                    getAdvertisements();
                } catch (UnsupportedEncodingException e) {
                    e.printStackTrace();
                }
            }
        }
        return view;
    }


    @Override
    public void onAttach(Context context) {
        super.onAttach(context);
        if (context instanceof OnListFragmentInteractionListener) {
            mListener = (OnListFragmentInteractionListener) context;
        } else {
            throw new RuntimeException(context.toString()
                    + " must implement OnListFragmentInteractionListener");
        }
    }

    @Override
    public void onDetach() {
        super.onDetach();
        mListener = null;
    }

    @Override
    public void onAdvertisementItemsReceived(int statusCode, JSONArray response) throws JSONException {
        if (statusCode == 200) {
            ArrayList<AdvertisementItemShortModel> list = new ArrayList<>();
            Gson gson = new Gson();
            Type advertisementType = new TypeToken<AdvertisementItemShortModel>(){}.getType();

            for (int i = 0; i < response.length(); i++) {
                AdvertisementItemShortModel model = gson.fromJson(response.get(i).toString(), advertisementType);
                list.add(model);
            }

           /* for(int i = 0; i < response.length(); i++){
                list.add(gson.fromJson(response.get(i).toString(), AdvertisementItemShortModel.class));
            }*/
            recyclerView.setAdapter(new AdvertisementItemRecyclerViewAdapter(list, mListener, getContext()));
            mMainActivity.onPreparedData();
        } else {
            ToastService.getInstance().showToast(this.getContext(), "Wystąpił błąd podczas pobierania ogłoszeń");
        }
    }

    public void getAdvertisements() throws UnsupportedEncodingException {
        AdvertisementItemsService.getInstance().GetAdvertisementItems(gps.getCoordinatesModel(), getActivity(), this);
    }

    public void setListener(MainActivity mainActivity) {
        mMainActivity = mainActivity;
    }

    /**
     * This interface must be implemented by activities that contain this
     * fragment to allow an interaction in this fragment to be communicated
     * to the activity and potentially other fragments contained in that
     * activity.
     * <p/>
     * See the Android Training lesson <a href=
     * "http://developer.android.com/training/basics/fragments/communicating.html"
     * >Communicating with Other Fragments</a> for more information.
     */
    public interface OnListFragmentInteractionListener {
        // TODO: Update argument type and name
        void onListFragmentInteraction(AdvertisementItemShortModel item);
    }

}
