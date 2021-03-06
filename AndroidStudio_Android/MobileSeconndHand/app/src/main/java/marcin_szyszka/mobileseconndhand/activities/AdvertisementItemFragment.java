package marcin_szyszka.mobileseconndhand.activities;

import android.content.Context;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.GridLayoutManager;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import org.json.JSONArray;
import org.json.JSONException;

import java.io.UnsupportedEncodingException;
import java.lang.reflect.Type;
import java.util.ArrayList;

import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.common.IAdvertisementItemsReceiver;
import marcin_szyszka.mobileseconndhand.models.AdvertisementItemShortModel;
import marcin_szyszka.mobileseconndhand.services.AdvertisementItemListActivityService;
import marcin_szyszka.mobileseconndhand.services.AdvertisementItemsService;
import marcin_szyszka.mobileseconndhand.services.GpsLocationService;
import marcin_szyszka.mobileseconndhand.services.ToastService;

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
    AdvertisementItemRecyclerViewAdapter fragmentListAdapter;

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
                    if (savedInstanceState != null) {
                        setFragmentAdapter(AdvertisementItemListActivityService
                                .getInstance()
                                .getStoredAdvertisementItemShortModelsList());
                        return view;
                    } else {
                        getAdvertisements();
                    }
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
            Type advertisementType = new TypeToken<AdvertisementItemShortModel>() {
            }.getType();

            for (int i = 0; i < response.length(); i++) {
                AdvertisementItemShortModel model = gson.fromJson(response.get(i).toString(), advertisementType);
                list.add(model);
            }
            setFragmentAdapter(list);
            mListener.onPreparedData();
        } else {
            ToastService.getInstance().showToast(this.getContext(), "Wystąpił błąd podczas pobierania ogłoszeń");
        }
    }

    private void setFragmentAdapter(ArrayList<AdvertisementItemShortModel> list) {
        if (fragmentListAdapter == null) {
            fragmentListAdapter = new AdvertisementItemRecyclerViewAdapter(list, mListener, getContext());
            recyclerView.setAdapter(fragmentListAdapter);
        } else {
            fragmentListAdapter.addItems(list);
        }
    }

    public void getAdvertisements() throws UnsupportedEncodingException {
        mListener.onStartDownloading();
        AdvertisementItemsService.getInstance().GetAdvertisementItems(gps.getCoordinatesModel(), getActivity(), this);
    }

    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        outState.putAll(outState);
        AdvertisementItemListActivityService
                .getInstance()
                .storeAdvertisementItemShortModelsList(fragmentListAdapter.getItems());
    }


    public interface OnListFragmentInteractionListener {
        void onListFragmentInteraction(AdvertisementItemShortModel item);

        void onInfinityScroll() throws UnsupportedEncodingException;

        void onPreparedData();

        void onStartDownloading();
    }

}
