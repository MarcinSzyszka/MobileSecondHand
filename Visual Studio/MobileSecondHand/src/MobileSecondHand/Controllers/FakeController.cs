using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileSecondHand.Controllers
{
    [Route("api/[controller]")]
    public class FakeController : Controller
    {
		Db.Services.MobileSecondHandContext context;
		public FakeController(Db.Services.MobileSecondHandContext context) {
			this.context = context;
		}
		[HttpGet]
		[Route("CreateFakeAdvertisements")]
		public void CreateFakeAdvertisements() {
			var fakeUser = new Db.Models.ApplicationUser { UserName = "test", PasswordHash = "test" };
			context.ApplicationUser.Add(fakeUser);

			var advertisement = new Db.Models.Advertisement.AdvertisementItem { IsActive = true, Title = "Spodnie klasyczne, materiałowe, rozm 40", Price = 40, Latitude = 52.0217376, Longitude = 20.8240181, User = fakeUser };
			var avdertisementPhoto = new Db.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement, IsMainPhoto = true, PhotoPath = "./images/advertisementItem/foty/spodnie 2.jpg" };
			advertisement.AdvertisementPhotos.Add(avdertisementPhoto);

			var advertisement2 = new Db.Models.Advertisement.AdvertisementItem { IsActive = true, Title = "Spodnie dżinsowe, rozm 44", Price = 55, Latitude = 52.0137376, Longitude = 20.8530181, User = fakeUser };
			var avdertisementPhoto2 = new Db.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement, IsMainPhoto = true, PhotoPath = "./images/advertisementItem/foty/spodnie.jpg" };
			advertisement2.AdvertisementPhotos.Add(avdertisementPhoto2);

			var advertisement3 = new Db.Models.Advertisement.AdvertisementItem { IsActive = true, Title = "Sukienka wieczorowa, rozm 38, TANIO", Price = 99, Latitude = 52.0417376, Longitude = 20.8540181, User = fakeUser };
			var avdertisementPhoto3 = new Db.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement, IsMainPhoto = true, PhotoPath = "./images/advertisementItem/foty/sukienka 2.jpg" };
			advertisement3.AdvertisementPhotos.Add(avdertisementPhoto3);

			var advertisement4 = new Db.Models.Advertisement.AdvertisementItem { IsActive = true, Title = "Sukienka \"mała czarna\", rozm 38, nowa!", Price = 69, Latitude = 52.0117376, Longitude = 20.8640181, User = fakeUser };
			var avdertisementPhoto4 = new Db.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement, IsMainPhoto = true, PhotoPath = "./images/advertisementItem/foty/sukienka.jpg" };
			advertisement4.AdvertisementPhotos.Add(avdertisementPhoto4);

			var advertisement5 = new Db.Models.Advertisement.AdvertisementItem { IsActive = true, Title = "Ciepły sweter rozm L. Jak nowy, tylko przymierzony!", Price = 49, Latitude = 52.0917376, Longitude = 20.8540181, User = fakeUser };
			var avdertisementPhoto5 = new Db.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement, IsMainPhoto = true, PhotoPath = "./images/advertisementItem/foty/sweter.jpg" };
			advertisement5.AdvertisementPhotos.Add(avdertisementPhoto5);

			context.AdvertisementItem.Add(advertisement);
			context.AdvertisementItem.Add(advertisement2);
			context.AdvertisementItem.Add(advertisement3);
			context.AdvertisementItem.Add(advertisement4);
			context.AdvertisementItem.Add(advertisement5);
			context.SaveChanges();
		}
    }
}
