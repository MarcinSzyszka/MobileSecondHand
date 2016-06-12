using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Holders;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.EventArgs;

namespace MobileSecondHand.App.Adapters {
	public class AdvertisementItemListAdapter : RecyclerView.Adapter {
		List<AdvertisementItemShort> advertisementItems;
		Activity context;
		BitmapOperationService bitmapOperationService;
		IAdvertisementsInfiniteScrollListener infiniteScrollListener;
		private int photoImageViewWitdth;
		private int photoImageViewHeight;

		public event EventHandler<ShowAdvertisementDetailsEventArgs> AdvertisementItemClick;
		public bool InfiniteScrollDisabled { get; set; }

		public int LastItemIndex { get; private set; }
		public AdvertisementItemViewHolder LastItemViewHolder { get; private set; }

		public AdvertisementItemListAdapter(Activity context, List<AdvertisementItemShort> advertisementItems, IAdvertisementsInfiniteScrollListener infiniteScrollListener) {
			this.advertisementItems = advertisementItems;
			this.context = context;
			this.bitmapOperationService = new BitmapOperationService();
			this.infiniteScrollListener = infiniteScrollListener;
			CalculateSizeForPhotoImageView();
		}

		public override int ItemCount
		{
			get { return this.advertisementItems.Count; }
		}

		public void AddAdvertisements(List<AdvertisementItemShort> advertisements) {
			CalculateSizeForPhotoImageView();
			this.advertisementItems.AddRange(advertisements);
			BindViewHolder(LastItemViewHolder, LastItemIndex);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
			var currentItem = this.advertisementItems[position];
			AdvertisementItemViewHolder vh = holder as AdvertisementItemViewHolder;
			vh.DistanceTextView.Text = String.Format("{0} km", currentItem.Distance);
			vh.TitleTextView.Text = currentItem.AdvertisementTitle;
			vh.PriceTextView.Text = String.Format("{0} z³", currentItem.AdvertisementPrice);
			vh.PhotoImageView.SetImageBitmap(bitmapOperationService.ResizeImage(currentItem.MainPhoto, vh.PhotoImageView.Width > 0 ? vh.PhotoImageView.Width : photoImageViewWitdth, vh.PhotoImageView.Height > 0 ? vh.PhotoImageView.Height : photoImageViewHeight));
			RaiseOnInfiniteScrollWhenItemIsLastInList(currentItem, vh);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AdvertisementItemRowView, parent, false);
			AdvertisementItemViewHolder vh = new AdvertisementItemViewHolder(itemView, OnAdvertisementItemClick);
			return vh;
		}

		private void CalculateSizeForPhotoImageView() {
			var metrics = context.Resources.DisplayMetrics;
			var width = metrics.WidthPixels - 20;
			this.photoImageViewWitdth = width;
			this.photoImageViewHeight = (int)(width * 0.8);
		}

		private int ConvertPixelsToDp(float pixelValue) {
			var dp = (int)((pixelValue) / context.Resources.DisplayMetrics.Density);
			return dp;
		}

		private void OnAdvertisementItemClick(int positionId) {
			if (AdvertisementItemClick != null) {
				AdvertisementItemClick(this, new ShowAdvertisementDetailsEventArgs { Id = this.advertisementItems[positionId].Id, Distance = this.advertisementItems[positionId].Distance });
			}
		}

		private void RaiseOnInfiniteScrollWhenItemIsLastInList(AdvertisementItemShort currentItem, AdvertisementItemViewHolder viewHolder) {
			if (this.advertisementItems.IndexOf(currentItem) == (this.advertisementItems.Count - 1) && !InfiniteScrollDisabled) {
				LastItemIndex = this.advertisementItems.IndexOf(currentItem);
				LastItemViewHolder = viewHolder;
				this.infiniteScrollListener.OnInfiniteScroll();
			}
		}

	}
}