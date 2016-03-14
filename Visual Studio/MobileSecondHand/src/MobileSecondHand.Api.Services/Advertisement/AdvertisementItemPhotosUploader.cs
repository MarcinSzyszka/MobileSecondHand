using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNet.Http;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Common.FIleNamesHelpers;
using MobileSecondHand.Common.PathHelpers;

namespace MobileSecondHand.Api.Services.Advertisement {
	public class AdvertisementItemPhotosUploader : IAdvertisementItemPhotosUploader {
		IAppFilesPathHelper appFilesPathHelper;
		IAppFilesNamesHelper appFilesNamesHelper;

		public AdvertisementItemPhotosUploader(IAppFilesPathHelper appFilesPathHelper, IAppFilesNamesHelper appFilesNamesHelper) {
			this.appFilesPathHelper = appFilesPathHelper;
			this.appFilesNamesHelper = appFilesNamesHelper;
		}

		public async Task<AdvertisementItemPhotosPaths> SaveAdvertisementPhotos(IFormFileCollection files) {
			var photosPathsModel = new AdvertisementItemPhotosPaths();
			var filesCount = files.Count;
			for (int i = 0; i < filesCount; i++) {
				using (Stream readStream = files.GetFile(this.appFilesNamesHelper.GetPhotoNameInForm(i)).OpenReadStream()) {
					var newFileName = this.appFilesNamesHelper.GetPhotoRandomUniqueName("jpg");
					var newFilePath = String.Concat(this.appFilesPathHelper.GetAdvertisementPhotosMainPath(), newFileName);
					using (FileStream fileStream = System.IO.File.Create(newFilePath)) {
						await readStream.CopyToAsync(fileStream);
						photosPathsModel.PhotosPaths.Add(newFilePath);
					}
					//first file (name ends with "0") will be main photo
					if (i==0) {
						var minImage = CreateMinPhoto(readStream);
						var newMinFilePath = String.Concat(this.appFilesPathHelper.GetAdvertisementMinPhotosMainPath(), newFileName);
						minImage.Save(newMinFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
					}
				}
			}
			return photosPathsModel;
		}

		private Bitmap CreateMinPhoto(Stream readStream) {
			var image = Image.FromStream(readStream);
			Bitmap bmpOriginal = new Bitmap(image, new Size(image.Width/6, image.Height/6));
			return bmpOriginal;
		}
	}
}
