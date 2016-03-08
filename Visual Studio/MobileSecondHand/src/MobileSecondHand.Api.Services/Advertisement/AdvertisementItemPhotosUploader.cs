using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
					var newFilePath = Path.Combine(this.appFilesPathHelper.GetAdvertisementPhotosMainPath(), this.appFilesNamesHelper.GetPhotoRandomUniqueName("jpg"));
					using (FileStream fileStream = System.IO.File.Create(newFilePath)) {
						await readStream.CopyToAsync(fileStream);
						photosPathsModel.PhotosPaths.Add(newFilePath);
					}
				}
			}
			return photosPathsModel;
		}
	}
}
