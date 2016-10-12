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

namespace MobileSecondHand.App.Holders
{
	public class AdvertisementPhotoViewHolder : RecyclerView.ViewHolder
	{
		public ImageView PhotoImageView { get; private set; }

		public AdvertisementPhotoViewHolder(View itemView) : base(itemView)
		{
			PhotoImageView = itemView.FindViewById<ImageView>(Resource.Id.advertPhotoView);
		}


	}
}