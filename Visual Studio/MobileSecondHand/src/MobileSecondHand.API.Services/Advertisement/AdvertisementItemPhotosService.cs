using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MobileSecondHand.API.Models.Advertisement;
using MobileSecondHand.COMMON.FIleNamesHelpers;
using MobileSecondHand.COMMON.PathHelpers;

namespace MobileSecondHand.API.Services.Advertisement {
	public class AdvertisementItemPhotosService : IAdvertisementItemPhotosService {
		IAppFilesPathHelper appFilesPathHelper;
		IAppFilesNamesHelper appFilesNamesHelper;

		public AdvertisementItemPhotosService(IAppFilesPathHelper appFilesPathHelper, IAppFilesNamesHelper appFilesNamesHelper) {
			this.appFilesPathHelper = appFilesPathHelper;
			this.appFilesNamesHelper = appFilesNamesHelper;
		}

		public async Task<AdvertisementItemPhotosPaths> SaveAdvertisementPhotos(IFormFileCollection files) {
			var photosPathsModel = new AdvertisementItemPhotosPaths();
			var filesCount = files.Count;
			for (int i = 0; i < filesCount; i++) {
				using (Stream readStream = files.GetFile(this.appFilesNamesHelper.GetPhotoNameInForm(i)).OpenReadStream()) {
					var newFileName = this.appFilesNamesHelper.GetPhotoRandomUniqueName("jpg");
					var newFilePath = String.Concat(this.appFilesPathHelper.GetAdvertisementPhotosMainPath(), "/", newFileName);
					using (FileStream fileStream = System.IO.File.Create(newFilePath)) {
						await readStream.CopyToAsync(fileStream);
						photosPathsModel.PhotosPaths.Add(newFilePath);
					}
					//first file (name ends with "0") will be main photo
					if (i==0) {
						var minImage = CreateMinPhoto(readStream);
						var newMinFilePath = String.Concat(this.appFilesPathHelper.GetAdvertisementMinPhotosMainPath(), "/", newFileName);
						minImage.Save(newMinFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
						photosPathsModel.PhotosPaths.Add(newMinFilePath);
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

		public async Task<byte[]> GetPhotoInBytes(string photoPath) {
			byte[] photo;
			using (var fs = new FileStream(photoPath, FileMode.Open)) {
				photo = new byte[fs.Length];
				await fs.ReadAsync(photo, 0, photo.Length);
			}

			return photo;
		}
	}
}
