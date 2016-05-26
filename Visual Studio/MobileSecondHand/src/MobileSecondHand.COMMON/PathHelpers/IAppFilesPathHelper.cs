using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.COMMON.PathHelpers {
	public interface IAppFilesPathHelper {
		string GetAdvertisementPhotosMainPath();
		string GetAdvertisementMinPhotosMainPath();
		bool IsMiniaturePhotoDirectory(string path);
	}
}
