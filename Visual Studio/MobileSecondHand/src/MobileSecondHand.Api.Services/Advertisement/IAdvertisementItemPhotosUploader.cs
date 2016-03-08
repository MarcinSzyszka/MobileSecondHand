using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using MobileSecondHand.Api.Models.Advertisement;

namespace MobileSecondHand.Api.Services.Advertisement
{
	public interface IAdvertisementItemPhotosUploader {
		Task<AdvertisementItemPhotosPaths> SaveAdvertisementPhotos(IFormFileCollection files);
	}
}
