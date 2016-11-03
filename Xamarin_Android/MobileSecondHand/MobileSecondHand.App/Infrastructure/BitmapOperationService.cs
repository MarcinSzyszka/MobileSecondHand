using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private int maxPhotoValue;

		public BitmapOperationService()
		{
			this.maxPhotoValue = 1000;
		}
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

		public async Task<byte[]> GetScaledDownPhotoByteArray(string path, bool isProfilePhoto = false)
		{
			var maxValue = maxPhotoValue;
			if (isProfilePhoto)
			{
				maxValue = 600;
			}
			var options = await GetBitmapOptionsOfImageAsync(path);
			float height = options.OutHeight;
			float width = options.OutWidth;
			if (height > width)
			{
				float divider = height / (float)maxValue;
				height = maxValue;
				width = (int)(width / divider);
			}
			else
			{
				float divider = width / (float)maxValue;
				width = maxValue;
				height = (int)(height / divider);
			}

			options.InSampleSize = CalculateInSampleSize(options, width, height);
			options.InJustDecodeBounds = false;
			var scaledBitmap = await BitmapFactory.DecodeFileAsync(path, options);

			byte[] imageArrayBytes;
			using (var stream = new MemoryStream())
			{
				scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
				imageArrayBytes = stream.ToArray();
			}

			return imageArrayBytes;
		}
		public async Task<Bitmap> GetScaledDownBitmapForDisplayAsync(byte[] imageByteArray)
		{
			var maxValue = maxPhotoValue;
			var options = await GetBitmapOptionsOfImageAsync(imageByteArray);
			float height = options.OutHeight;
			float width = options.OutWidth;
			if (height > width)
			{
				float divider = height / (float)maxValue;
				height = maxValue;
				width = (int)(width / divider);
			}
			else
			{
				float divider = width / (float)maxValue;
				width = maxValue;
				height = (int)(height / divider);
			}
			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize(options, width, height);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;

			return await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length, options);
		}

		public async Task<Bitmap> GetScaledDownBitmapForDisplayAsync(string path)
		{
			var maxValue = maxPhotoValue;
			var options = await GetBitmapOptionsOfImageAsync(path);
			float height = options.OutHeight;
			float width = options.OutWidth;
			if (height > width)
			{
				float divider = height / (float)maxValue;
				height = maxValue;
				width = (int)(width / divider);
			}
			else
			{
				float divider = width / (float)maxValue;
				width = maxValue;
				height = (int)(height / divider);
			}
			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize(options, width, height);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;

			return await BitmapFactory.DecodeFileAsync(path, options);
		}

		public async Task<Bitmap> GetNotScaledDownBitmapForDisplayAsync(string path)
		{
			var options = await GetBitmapOptionsOfImageAsync(path);
			float height = options.OutHeight;
			float width = options.OutWidth;

			// Calculate inSampleSize
			options.InSampleSize = CalculateInSampleSize(options, width, height);

			// Decode bitmap with inSampleSize set
			options.InJustDecodeBounds = false;

			return await BitmapFactory.DecodeFileAsync(path, options);
		}


		private int CalculateInSampleSize(BitmapFactory.Options options, float reqWidth, float reqHeight)
		{
			// Raw height and width of image
			float height = options.OutHeight;
			float width = options.OutWidth;
			double inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth)
			{
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
				{
					inSampleSize *= 2;
				}
			}

			return (int)inSampleSize;
		}

		private async Task<BitmapFactory.Options> GetBitmapOptionsOfImageAsync(byte[] imageByteArray)
		{
			BitmapFactory.Options options = GetBitmapOptions();

			// The result will be null because InJustDecodeBounds == true.
			Bitmap result = await BitmapFactory.DecodeByteArrayAsync(imageByteArray, 0, imageByteArray.Length, options);

			return options;
		}


		private async Task<BitmapFactory.Options> GetBitmapOptionsOfImageAsync(string path)
		{
			BitmapFactory.Options options = GetBitmapOptions();

			// The result will be null because InJustDecodeBounds == true.
			Bitmap result = await BitmapFactory.DecodeFileAsync(path, options);

			return options;
		}

		private static BitmapFactory.Options GetBitmapOptions()
		{
			return new BitmapFactory.Options
			{
				InJustDecodeBounds = true
			};
		}

	}
}