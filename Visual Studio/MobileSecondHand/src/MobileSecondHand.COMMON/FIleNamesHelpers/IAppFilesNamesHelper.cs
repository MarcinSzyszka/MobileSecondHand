using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.COMMON.FIleNamesHelpers
{
    public interface IAppFilesNamesHelper
    {
		string GetPhotoNameInForm(int fileNumber);
		string GetPhotoRandomUniqueName(string fileExtension);
	}
}
