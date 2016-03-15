using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Common.PathHelpers
{
    public class AppFilesPathHelper : IAppFilesPathHelper {
		const string ADVERTISEMENT_PHOTOS_MAIN_PATH = @"./images/advertisementItem/" + PHOTOS_DIRECTORY_NAME;
		public const string ADVERTISEMENT_MIN_PHOTOS_MAIN_PATH = ADVERTISEMENT_PHOTOS_MAIN_PATH + "/" + MIN_PHOTOS_DIRECTORY_NAME;
		const string MIN_PHOTOS_DIRECTORY_NAME = "min";
		const string PHOTOS_DIRECTORY_NAME = "photos";

		public string GetAdvertisementPhotosMainPath() {
			CreateDirectoryIfNotExists(ADVERTISEMENT_PHOTOS_MAIN_PATH);
			return ADVERTISEMENT_PHOTOS_MAIN_PATH;
		}
		public string GetAdvertisementMinPhotosMainPath() {
			CreateDirectoryIfNotExists(ADVERTISEMENT_MIN_PHOTOS_MAIN_PATH);
			return ADVERTISEMENT_MIN_PHOTOS_MAIN_PATH;
		}

		private void CreateDirectoryIfNotExists(string path) {
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
		}
	}
}
