using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.COMMON.PathHelpers
{
	public interface IAppFilesPathHelper
	{
		string GetAdvertisementMainPhotosPath();
		string GetAdvertisementMinPhotosMainPath();
		string GetUsersProfilesPhotosMainPath();
	}
}
