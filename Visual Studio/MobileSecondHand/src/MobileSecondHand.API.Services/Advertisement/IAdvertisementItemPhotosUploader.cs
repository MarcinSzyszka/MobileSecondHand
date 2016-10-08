using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MobileSecondHand.API.Models.Shared.Advertisements;

namespace MobileSecondHand.API.Services.Advertisement
{
	public interface IAdvertisementItemPhotosService {
		Task<AdvertisementItemPhotosPaths> SaveAdvertisementPhotos(IFormFileCollection files);
		Task<byte[]> GetPhotoInBytes(string photoPath);
	}
}
