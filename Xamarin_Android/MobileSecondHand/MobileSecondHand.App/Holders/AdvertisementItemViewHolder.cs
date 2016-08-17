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
		public ImageView SellerChatStateImageView { get; set; }
		public TextView AdvertisementKindTextView { get; set; }
		public com.refractored.fab.FloatingActionButton DeleteAdvertisementFab { get; private set; }

		public AdvertisementItemViewHolder(View itemView, Action<int> clickAction, Action<int> deleteAdvertisementAction) : base(itemView) {
			DistanceTextView = itemView.FindViewById<TextView>(Resource.Id.distanceTextView);
			TitleTextView = itemView.FindViewById<TextView>(Resource.Id.advertisementOnListTitle);
			PriceTextView = itemView.FindViewById<TextView>(Resource.Id.advertisementPriceListTextView);
			PhotoImageView = itemView.FindViewById<ImageView>(Resource.Id.advertisementPhotoImageView);
			SellerChatStateImageView = itemView.FindViewById<ImageView>(Resource.Id.sellerChatState);
			AdvertisementKindTextView = itemView.FindViewById<TextView>(Resource.Id.advertisementKind);
			DeleteAdvertisementFab = itemView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fab_remove_advertisement);
			DeleteAdvertisementFab.BringToFront();


			DeleteAdvertisementFab.Click += (s, e) => deleteAdvertisementAction(Position);
			itemView.Click += (s, e) => clickAction(Position);
		}

	}
}