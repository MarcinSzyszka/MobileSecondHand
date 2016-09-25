using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using MobileSecondHand.App.Activities;

namespace MobileSecondHand.App.Infrastructure
{
	public class BitmapOperationService
	{

		public string SavePhotoFromUriAndReturnPhysicalPath(Android.Net.Uri contentURI, Java.IO.File file, Context ctx)
		{
			try
			{

				var contentResolver = ctx.ContentResolver;
				var stream = contentResolver.OpenInputStream(contentURI);
				using (OutputStream output = new FileOutputStream(file))
				{
					var memStream = new MemoryStream();
					stream.CopyTo(memStream);
					var buffer = memStream.ToArray();
					
					output.Write(buffer);
				}

				return file.AbsolutePath;
			}
			catch (Exception exc)
			{

				return String.Empty;
			}
		}

		public Bitmap GetBitmap(string imagePath)
		{
			Bitmap bitmap = BitmapFactory.DecodeFile(imagePath);

			return bitmap;
		}

		public Bitmap GetBitmap(byte[] imageByteArray)
		{
			int offset = 0;

			Bitmap bitmap = BitmapFactory.DecodeByteArray(imageByteArray, offset, imageByteArray.Length);

			return bitmap;
		}

		public byte[] ResizeImageAndGetByteArray(byte[] imageByteArray)
		{
			int offset = 0;
			var maxValue = 1500;

			var decodedBitmap = BitmapFactory.DecodeByteArray(imageByteArray, offset, imageByteArray.Length);

			var width = 0;
			var height = 0;

			if (decodedBitmap.Height > decodedBitmap.Width)
			{
				double divider = (double)decodedBitmap.Height / (double)maxValue;
				height = maxValue;
				width = (int)((double)decodedBitmap.Width / divider);
			}
			else
			{
				double divider = (double)decodedBitmap.Width / (double)maxValue;
				width = maxValue;
				height = (int)((double)decodedBitmap.Height / divider);
			}

			var bitmapScalled = Bitmap.CreateScaledBitmap(decodedBitmap, width, height, true);
			decodedBitmap.Recycle();

			byte[] imageArrayBytes;
			using (var stream = new MemoryStream())
			{
				bitmapScalled.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
				imageArrayBytes = stream.ToArray();
			}

			return imageArrayBytes;
		}

		public Bitmap ResizeImageAndGetBitMap(string filePath)
		{
			var maxValue = 1500;

			var decodedBitmap = BitmapFactory.DecodeFile(filePath);

			var width = 0;
			var height = 0;

			if (decodedBitmap.Height > decodedBitmap.Width)
			{
				double divider = (double)decodedBitmap.Height / (double)maxValue;
				height = maxValue;
				width = (int)((double)decodedBitmap.Width / divider);
			}
			else
			{
				double divider = (double)decodedBitmap.Width / (double)maxValue;
				width = maxValue;
				height = (int)((double)decodedBitmap.Height / divider);
			}

			var bitmapScalled = Bitmap.CreateScaledBitmap(decodedBitmap, width, height, true);
			decodedBitmap.Recycle();

			return bitmapScalled;
		}


	}
}