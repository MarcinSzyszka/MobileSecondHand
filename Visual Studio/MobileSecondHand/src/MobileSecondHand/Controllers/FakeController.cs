﻿using System;
using Microsoft.AspNetCore.Mvc;
using MobileSecondHand.DB.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MobileSecondHand.Controllers
{
	[Route("api/[controller]")]
	public class FakeController : Controller
	{
		MobileSecondHandContext context;
		public FakeController(MobileSecondHandContext context)
		{
			this.context = context;
		}
		[HttpGet]
		[Route("CreateFakeAdvertisements")]
		public void CreateFakeAdvertisements()
		{
			AddFiveAdvertisements();
		}

		[HttpGet]
		[Route("CreateALotOfFakeAdvertisements")]
		public void CreateALotOfFakeAdvertisements()
		{
			for (int i = 0; i < 1000; i++)
			{
				AddFiveAdvertisements();
			}
		}

		private void AddFiveAdvertisements()
		{
			var fakeUser = new DB.Models.Authentication.ApplicationUser { UserName = "test", PasswordHash = "test" };
			context.ApplicationUser.Add(fakeUser);

			var advertisement = new DB.Models.Advertisement.AdvertisementItem { CategoryId = 1,  ExpirationDate = DateTime.Now.AddDays(7), Title = "Spodnie klasyczne, materiałowe, rozm 40", Price = 40, Latitude = 52.0217376, Longitude = 20.8240181, User = fakeUser };
			var avdertisementPhoto = new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement, IsMainPhoto = true, PhotoName = "./wwwroot/images/advertisementItem/foty/spodnie 2min.jpg" };
			advertisement.AdvertisementPhotos.Add(avdertisementPhoto);
			advertisement.AdvertisementPhotos.Add(new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement, IsMainPhoto = false, PhotoName = "./wwwroot/images/advertisementItem/foty/spodnie 2.jpg" });

			var advertisement2 = new DB.Models.Advertisement.AdvertisementItem { CategoryId = 1,  ExpirationDate = DateTime.Now.AddDays(7), Title = "Spodnie dżinsowe, rozm 44", Price = 55, Latitude = 52.0137376, Longitude = 20.8530181, User = fakeUser };
			var avdertisementPhoto2 = new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement2, IsMainPhoto = true, PhotoName = "./wwwroot/images/advertisementItem/foty/spodniemin.jpg" };
			advertisement2.AdvertisementPhotos.Add(avdertisementPhoto2);
			advertisement2.AdvertisementPhotos.Add(new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement2, IsMainPhoto = false, PhotoName = "./wwwroot/images/advertisementItem/foty/spodnie.jpg" });

			var advertisement3 = new DB.Models.Advertisement.AdvertisementItem { CategoryId = 1,  ExpirationDate = DateTime.Now.AddDays(7), Title = "Sukienka wieczorowa, rozm 38, TANIO", Price = 99, Latitude = 52.0417376, Longitude = 20.8540181, User = fakeUser };
			var avdertisementPhoto3 = new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement3, IsMainPhoto = true, PhotoName = "./wwwroot/images/advertisementItem/foty/sukienka 2min.jpg" };
			advertisement3.AdvertisementPhotos.Add(avdertisementPhoto3);
			advertisement3.AdvertisementPhotos.Add(new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement3, IsMainPhoto = false, PhotoName = "./wwwroot/images/advertisementItem/foty/sukienka 2.jpg" });

			var advertisement4 = new DB.Models.Advertisement.AdvertisementItem { CategoryId = 1,  ExpirationDate = DateTime.Now.AddDays(7), Title = "Sukienka \"mała czarna\", rozm 38, nowa!", Price = 69, Latitude = 52.0117376, Longitude = 20.8640181, User = fakeUser };
			var avdertisementPhoto4 = new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement4, IsMainPhoto = true, PhotoName = "./wwwroot/images/advertisementItem/foty/sukienkamin.png" };
			advertisement4.AdvertisementPhotos.Add(avdertisementPhoto4);
			advertisement4.AdvertisementPhotos.Add(new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement4, IsMainPhoto = false, PhotoName = "./wwwroot/images/advertisementItem/foty/sukienka.png" });

			var advertisement5 = new DB.Models.Advertisement.AdvertisementItem { CategoryId = 1, ExpirationDate = DateTime.Now.AddDays(7), Title = "Ciepły sweter rozm L. Jak nowy, tylko przymierzony!", Price = 49, Latitude = 52.0917376, Longitude = 20.8540181, User = fakeUser };
			var avdertisementPhoto5 = new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement5, IsMainPhoto = true, PhotoName = "./wwwroot/images/advertisementItem/foty/swetermin.jpg" };
			advertisement5.AdvertisementPhotos.Add(avdertisementPhoto5);
			advertisement5.AdvertisementPhotos.Add(new DB.Models.Advertisement.AdvertisementPhoto { AdvertisementItem = advertisement5, IsMainPhoto = false, PhotoName = "./wwwroot/images/advertisementItem/foty/sweter.jpg" });

			context.AdvertisementItem.Add(advertisement);
			context.AdvertisementItem.Add(advertisement2);
			context.AdvertisementItem.Add(advertisement3);
			context.AdvertisementItem.Add(advertisement4);
			context.AdvertisementItem.Add(advertisement5);
			context.SaveChanges();
		}

	}
}
