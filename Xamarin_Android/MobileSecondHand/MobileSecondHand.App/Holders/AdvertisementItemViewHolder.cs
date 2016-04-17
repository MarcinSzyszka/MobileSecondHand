using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MobileSecondHand.App.Holders {
	public class AdvertisementItemViewHolder : RecyclerView.ViewHolder {
		public TextView DistanceTextView { get; set; }
		public ImageView PhotoImageView { get; set; }
		public TextView PriceTextView { get; set; }
		public TextView TitleTextView { get; set; }

		public AdvertisementItemViewHolder(View itemView, Action<int> clickAction) : base(itemView) {
			DistanceTextView = itemView.FindViewById<TextView>(Resource.Id.distanceTextView);
			TitleTextView = itemView.FindViewById<TextView>(Resource.Id.advertisementOnListTitle);
			PriceTextView = itemView.FindViewById<TextView>(Resource.Id.advertisementPriceListTextView);
			PhotoImageView = itemView.FindViewById<ImageView>(Resource.Id.advertisementPhotoImageView);

			itemView.Click += (s, e) => clickAction(Position);
		}

	}
}