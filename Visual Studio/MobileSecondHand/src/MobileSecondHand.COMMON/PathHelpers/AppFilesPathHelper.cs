using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Config;

namespace MobileSecondHand.COMMON.PathHelpers
{
	public class AppFilesPathHelper : IAppFilesPathHelper
	{
		private AppConfiguration config;
		string UserProfilePhotosDirectory
		{
			get
			{
				return Path.Combine(this.config.FileRepositoryPath, "UserProfilesPhotos");
			}
		}
		string AdvertisementPhotosDirectory
		{
			get
			{
				return Path.Combine(this.config.FileRepositoryPath, "AdvertisementsPhotos");
			}
		}
		string AdvertisemenPhotosMinDirectory
		{
			get
			{
				return Path.Combine(this.config.FileRepositoryPath, "AdvertisementsPhotosMin");
			}
		}
	

		public AppFilesPathHelper(AppConfiguration appConfig)
		{
			this.config = appConfig;
		}
		public string GetAdvertisementMainPhotosPath()
		{
			CreateDirectoryIfNotExists(AdvertisementPhotosDirectory);
			return AdvertisementPhotosDirectory;
		}
		public string GetAdvertisementMinPhotosMainPath()
		{
			CreateDirectoryIfNotExists(AdvertisemenPhotosMinDirectory);
			return AdvertisemenPhotosMinDirectory;
		}
		public string GetUsersProfilesPhotosMainPath()
		{
			CreateDirectoryIfNotExists(UserProfilePhotosDirectory);
			return UserProfilePhotosDirectory;
		}

		private void CreateDirectoryIfNotExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}


	}
}
