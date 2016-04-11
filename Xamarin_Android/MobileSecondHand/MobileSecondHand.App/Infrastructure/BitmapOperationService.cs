using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileSecondHand.App.Infrastructure {
	public class BitmapOperationService {
		public Bitmap ResizeImage(string imagePath, int width, int height) {
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeFile(imagePath, bmOptions);

			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeFile(imagePath, bmOptions);
			return bitmap;
		}

		public Bitmap ResizeImage(byte[] imageByteArray, int width, int height) {
			int offset = 0;
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeByteArray(imageByteArray, offset, imageByteArray.Length, bmOptions);

			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByteArray, 0, imageByteArray.Length, bmOptions);
			return bitmap;
		}

		public Bitmap ResizeImage(Resources res, int resourceId, int width, int height) {
			// Get the dimensions of the bitmap
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeResource(res, resourceId);

			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeResource(res, resourceId);
			return bitmap;
		}

		private BitmapFactory.Options GetBitmapOptions() {
			BitmapFactory.Options bmOptions = new BitmapFactory.Options();
			bmOptions.InJustDecodeBounds = true;
			return bmOptions;
		}

		private static void scaleImage(int width, int height, BitmapFactory.Options bmOptions) {
			if (width > 0 && height > 0) {
				int photoW = bmOptions.OutWidth;
				int photoH = bmOptions.OutHeight;

				int scaleFactor = Math.Min(photoW / width, photoH / height);
				bmOptions.InSampleSize = scaleFactor;
			}

			bmOptions.InJustDecodeBounds = false;
			bmOptions.InPurgeable = true;
		}
	}
}