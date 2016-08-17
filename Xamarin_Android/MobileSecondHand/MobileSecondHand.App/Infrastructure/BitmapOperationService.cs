using System;
using System.Collections.Generic;
using System.IO;
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

namespace MobileSecondHand.App.Infrastructure
{
	public class BitmapOperationService
	{
		public Bitmap ResizeImage(string imagePath, int width, int height)
		{
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeFile(imagePath, bmOptions);

			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeFile(imagePath, bmOptions);
			return bitmap;
		}

		public Bitmap ResizeImage(byte[] imageByteArray, int width, int height)
		{
			int offset = 0;
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeByteArray(imageByteArray, offset, imageByteArray.Length, bmOptions);

			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByteArray, 0, imageByteArray.Length, bmOptions);
			return bitmap;
		}

		public Bitmap ResizeImageByWidth(byte[] imageByteArray, int width)
		{
			int offset = 0;
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeByteArray(imageByteArray, offset, imageByteArray.Length, bmOptions);

			//var divider = bmOptions.OutWidth / width;
			//var height = bmOptions.OutHeight;
			//if (divider > 0)
			//{
			//	height = bmOptions.OutHeight / divider;
			//}
			double divider = (double)bmOptions.OutWidth / (double)width;
			var height = bmOptions.OutHeight;

			//if (divider >= 1)
			//{
				height = (int)((double)bmOptions.OutHeight / divider);
			//}
			//else
			//{
			//	height = (int)(((double)bmOptions.OutHeight) * divider);
			//}



			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByteArray, 0, imageByteArray.Length, bmOptions);
			return bitmap;
		}

		public byte[] ResizeImageAndGetByteArray(byte[] imageByteArray, int maxValue)
		{
			int offset = 0;
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeByteArray(imageByteArray, offset, imageByteArray.Length, bmOptions);
			var width = 0;
			var height = 0;

			if (bmOptions.OutHeight > bmOptions.OutWidth)
			{
				double divider = (double)bmOptions.OutHeight / (double)maxValue;
				height = maxValue;
				width = (int)((double)bmOptions.OutWidth / divider);
			}
			else
			{
				double divider = (double)bmOptions.OutWidth / (double)maxValue;
				width = maxValue;
				height = (int)((double)bmOptions.OutHeight / divider);
			}

			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByteArray, 0, imageByteArray.Length, bmOptions);

			byte[] imageArrayBytes;
			using (var stream = new MemoryStream())
			{
				bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
				imageArrayBytes = stream.ToArray();
			}

			return imageArrayBytes;
		}

		public Bitmap ResizeImage(Resources res, int resourceId, int width, int height)
		{
			// Get the dimensions of the bitmap
			BitmapFactory.Options bmOptions = GetBitmapOptions();
			BitmapFactory.DecodeResource(res, resourceId);

			scaleImage(width, height, bmOptions);

			Bitmap bitmap = BitmapFactory.DecodeResource(res, resourceId);
			return bitmap;
		}

		private BitmapFactory.Options GetBitmapOptions()
		{
			BitmapFactory.Options bmOptions = new BitmapFactory.Options();
			bmOptions.InJustDecodeBounds = true;
			return bmOptions;
		}

		private static void scaleImage(int width, int height, BitmapFactory.Options bmOptions)
		{
			if (width > 0 && height > 0)
			{
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