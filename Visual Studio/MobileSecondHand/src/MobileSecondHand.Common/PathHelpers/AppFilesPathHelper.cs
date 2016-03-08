using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Common.PathHelpers
{
    public class AppFilesPathHelper : IAppFilesPathHelper {
		const string ADVERTISEMENT_PHOTOS_MAIN_PATH = @"./images/";

		public string GetAdvertisementPhotosMainPath() {
			return ADVERTISEMENT_PHOTOS_MAIN_PATH;
		}
	}
}
