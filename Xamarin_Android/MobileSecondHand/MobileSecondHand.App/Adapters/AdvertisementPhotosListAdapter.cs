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
using ImageViews.Photo;
using MobileSecondHand.App.Holders;
using MobileSecondHand.App.Infrastructure;

namespace MobileSecondHand.App.Adapters
{
	public class AdvertisementPhotosListAdapter : RecyclerView.Adapter
	{
		private BitmapOperationService bitmapOperationService;
		private bool handleOnClick;
		private List<byte[]> photos;
		private bool zoomable;

		public event EventHandler<int> PhotoClicked;

		public AdvertisementPhotosListAdapter(List<byte[]> photos, bool handleOnClick = false, bool zoomable = false)
		{
			this.photos = photos;
			this.bitmapOperationService = new BitmapOperationService();
			this.handleOnClick = handleOnClick;
			this.zoomable = zoomable;
		}
		public override int ItemCount
		{
			get
			{
				return photos.Count;
			}
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var photo = this.photos[position];
			AdvertisementPhotoViewHolder vh = holder as AdvertisementPhotoViewHolder;
			if (handleOnClick)
			{
				vh.SetActionOnClick(() =>
				{
					if (PhotoClicked != null)
					{
						PhotoClicked(this, position);
					}
				});
			}
			vh.PhotoImageView.SetImageBitmap(bitmapOperationService.GetBitmap(photo));
			if (zoomable)
			{
				var attacher = new PhotoViewAttacher(vh.PhotoImageView);
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.advertisementPhotoRowView, parent, false);
			AdvertisementPhotoViewHolder vh = new AdvertisementPhotoViewHolder(itemView);

			return vh;
		}
	}
}