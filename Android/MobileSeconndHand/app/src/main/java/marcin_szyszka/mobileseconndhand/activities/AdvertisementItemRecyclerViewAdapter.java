package marcin_szyszka.mobileseconndhand.activities;

import android.content.Context;
import android.content.res.Resources;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import cz.msebera.android.httpclient.client.cache.Resource;
import marcin_szyszka.mobileseconndhand.R;
import marcin_szyszka.mobileseconndhand.models.AdvertisementItemShortModel;
import marcin_szyszka.mobileseconndhand.services.BitmapOperationService;

import java.util.List;

public class AdvertisementItemRecyclerViewAdapter extends RecyclerView.Adapter<AdvertisementItemRecyclerViewAdapter.ViewHolder> {
private Context context;
    private final List<AdvertisementItemShortModel> mValues;
    private final AdvertisementItemFragment.OnListFragmentInteractionListener mListener;

    public AdvertisementItemRecyclerViewAdapter(List<AdvertisementItemShortModel> items, AdvertisementItemFragment.OnListFragmentInteractionListener listener, Context context) {
        mValues = items;
        mListener = listener;
        this.context = context;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.fragment_item, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(final ViewHolder holder, int position) {
        holder.mItem = mValues.get(position);
        holder.mTitleTextView.setText(mValues.get(position).AdvertisementTitle);
        holder.mPriceTextView.setText(mValues.get(position).AdvertisementPrice + " zł");
        holder.mDistanceTextView.setText(String.valueOf(mValues.get(position).Distance) + " km");
        holder.mAdvertisementMainPhoto.setImageBitmap(BitmapOperationService.ResizeImage(context.getResources(), R.mipmap.add_photo_image, holder.mAdvertisementMainPhoto.getWidth(), holder.mAdvertisementMainPhoto.getHeight()));

        holder.mView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (null != mListener) {
                    // Notify the active callbacks interface (the activity, if the
                    // fragment is attached to one) that an item has been selected.
                    mListener.onListFragmentInteraction(holder.mItem);
                }
            }
        });
    }

    @Override
    public int getItemCount() {
        return mValues.size();
    }

    public class ViewHolder extends RecyclerView.ViewHolder {
        public final View mView;
        public final TextView mTitleTextView;
        public final TextView mPriceTextView;
        public final TextView mDistanceTextView;
        private final ImageView mAdvertisementMainPhoto;
        public AdvertisementItemShortModel mItem;

        public ViewHolder(View view) {
            super(view);
            mView = view;
            mTitleTextView = (TextView) view.findViewById(R.id.advertisementTitleTextView);
            mPriceTextView = (TextView) view.findViewById(R.id.advertisementPriceTextView);
            mDistanceTextView = (TextView) view.findViewById(R.id.advertisementDistanceTextView);
            mAdvertisementMainPhoto = (ImageView) view.findViewById(R.id.advertisementMinPhoto);
        }

        @Override
        public String toString() {
            return super.toString() + " '" + mTitleTextView.getText() + "'";
        }
    }
}