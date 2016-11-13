using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.COMMON.FIleNamesHelpers;
using MobileSecondHand.COMMON.PathHelpers;

namespace MobileSecondHand.API.Services.Photos
{
	public class PhotosService : IPhotosService
	{
		double minPhotoMaxWidth = 300.0;
		double resizedPhotoMaxSizeValue = 800.0;
		double profilePhotoMaxSize = 150;
		IAppFilesPathHelper appFilesPathHelper;
		IAppFilesNamesHelper appFilesNamesHelper;

		public PhotosService(IAppFilesPathHelper appFilesPathHelper, IAppFilesNamesHelper appFilesNamesHelper)
		{
			this.appFilesPathHelper = appFilesPathHelper;
			this.appFilesNamesHelper = appFilesNamesHelper;
		}

		public AdvertisementItemPhotosNames SaveAdvertisementPhotos(IFormFileCollection files)
		{
			var photosPathsModel = new AdvertisementItemPhotosNames();
			var filesCount = files.Count;
			for (int i = 0; i < filesCount; i++)
			{
				using (Stream readStream = files.GetFile(this.appFilesNamesHelper.GetPhotoNameInForm(i)).OpenReadStream())
				{
					var newFileName = this.appFilesNamesHelper.GetPhotoRandomUniqueName("jpg");
					var newFilePath = String.Concat(this.appFilesPathHelper.GetAdvertisementMainPhotosPath(), "/", newFileName);
					var resizedPhoto = ResizePhoto(readStream);
					resizedPhoto.Save(newFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
					photosPathsModel.PhotosNames.Add(newFileName);
					//first file (name ends with "0") will be main photo
					if (i == 0)
					{
						var minPhotoName = "mini" + newFileName;
						var minImage = CreateMinPhoto(readStream);
						var newMinFilePath = Path.Combine(this.appFilesPathHelper.GetAdvertisementMinPhotosMainPath(), minPhotoName);
						minImage.Save(newMinFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
						photosPathsModel.PhotosNames.Add(minPhotoName);
					}
				}
			}
			return photosPathsModel;
		}



		public string SaveUserProfilePhoto(IFormFileCollection files)
		{
			using (Stream readStream = files.GetFile("profilePhoto").OpenReadStream())
			{
				var minImage = ResizePhoto(readStream, true);
				var newFileName = this.appFilesNamesHelper.GetPhotoRandomUniqueName("jpg");
				var newFilePath = Path.Combine(this.appFilesPathHelper.GetUsersProfilesPhotosMainPath(), newFileName);
				minImage.Save(newFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);

				return newFileName;
			}
		}




		public async Task<byte[]> GetAdvertisementMinPhotoInBytes(string photoName)
		{
			var path = Path.Combine(this.appFilesPathHelper.GetAdvertisementMinPhotosMainPath(), photoName);
			return await GetBytesArray(path);
		}

		public async Task<byte[]> GetAdvertisementMainPhotoInBytes(string photoName)
		{
			var path = Path.Combine(this.appFilesPathHelper.GetAdvertisementMainPhotosPath(), photoName);
			return await GetBytesArray(path);
		}

		public async Task<byte[]> GetUserProfilePhotoInBytes(string photoName)
		{
			if (String.IsNullOrEmpty(photoName))
			{
				return null;
			}
			var path = Path.Combine(this.appFilesPathHelper.GetUsersProfilesPhotosMainPath(), photoName);
			return await GetBytesArray(path);
		}

		private async Task<byte[]> GetBytesArray(string photoName)
		{
			byte[] photo;
			using (var fs = new FileStream(photoName, FileMode.Open))
			{
				photo = new byte[fs.Length];
				await fs.ReadAsync(photo, 0, photo.Length);
			}

			return photo;
		}


		private Bitmap ResizePhoto(Stream readStream, bool isUserProfile = false)
		{
			var maxValue = isUserProfile ? profilePhotoMaxSize : resizedPhotoMaxSizeValue;
			var image = Image.FromStream(readStream);
			double divider = 1.0;
			var imageWidth = (double)image.Width;
			var imageHeight = (double)image.Height;
			if (imageHeight > imageWidth)
			{
				divider = imageHeight / maxValue;
			}
			else
			{
				divider = imageWidth / maxValue;
			}

			Bitmap bmpOriginal = new Bitmap(image, new Size((int)(imageWidth / divider), (int)(imageHeight / divider)));
			return bmpOriginal;
		}
		private Bitmap CreateMinPhoto(Stream readStream)
		{
			var image = Image.FromStream(readStream);
			double divider = 1.0;
			var imageWidth = (double)image.Width;
			var imageHeight = (double)image.Height;
			if (image.Width > minPhotoMaxWidth)
			{
				divider = imageWidth / minPhotoMaxWidth;
			}
			Bitmap bmpOriginal = new Bitmap(image, new Size((int)(imageWidth / divider), (int)(imageHeight / divider)));
			return bmpOriginal;
		}


	}
}
