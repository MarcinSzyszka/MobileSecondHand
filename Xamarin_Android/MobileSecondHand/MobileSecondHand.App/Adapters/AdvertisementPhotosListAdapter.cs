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
		private List<byte[]> photos;

		public event EventHandler<int> PhotoClicked;

		public AdvertisementPhotosListAdapter(List<byte[]> photos)
		{
			this.photos = photos;
			this.bitmapOperationService = new BitmapOperationService();
		}
		public override int ItemCount
		{
			get
			{
				return photos.Count;
			}
		}

		public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var photo = this.photos[position];
			AdvertisementPhotoViewHolder vh = holder as AdvertisementPhotoViewHolder;
			vh.SetActionOnClick(() =>
			{
				if (PhotoClicked != null)
				{
					PhotoClicked(this, position);
				}
			});
			vh.PhotoImageView.SetImageBitmap(await bitmapOperationService.GetScaledDownBitmapForDisplayAsync(photo));
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.advertisementPhotoRowView, parent, false);
			AdvertisementPhotoViewHolder vh = new AdvertisementPhotoViewHolder(itemView);

			return vh;
		}
	}
}