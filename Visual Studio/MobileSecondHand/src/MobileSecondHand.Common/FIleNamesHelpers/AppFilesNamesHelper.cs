using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Common.FIleNamesHelpers
{
    public class AppFilesNamesHelper : IAppFilesNamesHelper
    {
		const string PHOTO_NAME_IN_FORM = "photo";

		public string GetPhotoNameInForm(int fileNumber) {
			return String.Format("{0}{1}", PHOTO_NAME_IN_FORM, fileNumber);
		}

		public string GetPhotoRandomUniqueName(string fileExtension) {
			return String.Format("{0}.{1}", Guid.NewGuid().ToString(), fileExtension);
		}

	}
}
