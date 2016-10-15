using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using ImageViews.Photo;
using Java.Lang;
using MobileSecondHand.App.Infrastructure;

namespace MobileSecondHand.App.Adapters
{
	public class PhotosViewPagerAdapter : PagerAdapter
	{
		private List<byte[]> photos;
		private BitmapOperationService bitmapOperationService;

		public override int Count
		{
			get
			{
				return photos.Count;
			}
		}

		public PhotosViewPagerAdapter(List<byte[]> photos)
		{
			this.photos = photos;
			this.bitmapOperationService = new BitmapOperationService();
		}

		public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
		{
			return view == objectValue;
		}

		public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
		{
			PhotoView photoView = new PhotoView(container.Context);
			photoView.SetImageBitmap(bitmapOperationService.GetBitmap(photos[position]));

			// Now just add PhotoView to ViewPager and return it
			container.AddView(photoView, ViewPager.LayoutParams.MatchParent, ViewPager.LayoutParams.MatchParent);

			return photoView;
		}

		public override void DestroyItem(View container, int position, Java.Lang.Object objectValue)
		{
			base.DestroyItem(container, position, objectValue);
		}
	}
}