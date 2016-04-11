using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;

namespace MobileSecondHand.App.Adapters {
	public class AdvertisementItemListAdapter : BaseAdapter<AdvertisementItemShort> {
		List<AdvertisementItemShort> advertisementItems;
		Activity context;
		BitmapOperationService bitmapOperationService;

		public AdvertisementItemListAdapter(Activity context, List<AdvertisementItemShort> advertisementItems) {
			this.advertisementItems = advertisementItems;
			this.context = context;
			this.bitmapOperationService = new BitmapOperationService();
		}

		public override AdvertisementItemShort this[int position]
		{
			get { return advertisementItems[position]; }
		}

		public override int Count
		{
			get { return this.advertisementItems.Count; }
		}

		public override long GetItemId(int position) {
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent) {
			var currentItem = this[position];

			if (convertView == null) {
				convertView = this.context.LayoutInflater.Inflate(Resource.Layout.AdvertisementItemRowView, null);
			}
			SetupRowView(convertView, currentItem);
			return convertView;
		}

		private void SetupRowView(View convertView, AdvertisementItemShort currentItem) {
			var distanceTextView = convertView.FindViewById<TextView>(Resource.Id.distanceTextView);
			var titleTextView = convertView.FindViewById<TextView>(Resource.Id.advertisementOnListTitle);
			var priceTextView = convertView.FindViewById<TextView>(Resource.Id.advertisementPriceListTextView);
			var photoImageView = convertView.FindViewById<ImageView>(Resource.Id.advertisementPhotoImageView);

			distanceTextView.Text = String.Format("{0} km", currentItem.Distance);
			distanceTextView.Text = currentItem.AdvertisementTitle;
			priceTextView.Text = currentItem.AdvertisementPrice.ToString();
			photoImageView.SetImageBitmap(bitmapOperationService.ResizeImage(currentItem.MainPhoto, photoImageView.Width, photoImageView.Height));
		}
	}
}