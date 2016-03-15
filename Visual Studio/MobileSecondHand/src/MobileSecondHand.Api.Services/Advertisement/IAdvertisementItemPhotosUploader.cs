using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Db.Models.Advertisement;

namespace MobileSecondHand.Api.Services.Advertisement
{
	public interface IAdvertisementItemPhotosService {
		Task<AdvertisementItemPhotosPaths> SaveAdvertisementPhotos(IFormFileCollection files);
		Task<byte[]> GetPhotoInBytes(string photoPath);
	}
}
