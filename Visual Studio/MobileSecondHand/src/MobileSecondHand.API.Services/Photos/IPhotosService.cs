using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MobileSecondHand.API.Models.Shared.Advertisements;

namespace MobileSecondHand.API.Services.Photos
{
	public interface IPhotosService
	{
		AdvertisementItemPhotosNames SaveAdvertisementPhotos(IFormFileCollection files);
		string SaveUserProfilePhoto(IFormFileCollection files);
		Task<byte[]> GetAdvertisementMinPhotoInBytes(string photoName);
		Task<byte[]> GetAdvertisementMainPhotoInBytes(string photoName);
		Task<byte[]> GetUserProfilePhotoInBytes(string photoName);
	}
}
