using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileSecondHand.API.Models.CustomResponsesModels;
using MobileSecondHand.API.Services.Advertisement;
using MobileSecondHand.API.Services.Authentication;
using Microsoft.Extensions.Logging;
using MobileSecondHand.API.Models.Shared;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.API.Services.Photos;

namespace MobileSecondHand.Controllers
{
	[Microsoft.AspNetCore.Authorization.Authorize("Bearer")]
	[Route("api/[controller]")]
	public class AdvertisementItemController : Controller
	{
		IPhotosService advertisementItemPhotosUploader;
		IAdvertisementItemService advertisementItemService;
		IIdentityService identityService;
		private ILogger logger;

		public AdvertisementItemController(IPhotosService advertisementItemPhotosUploader, IAdvertisementItemService advertisementItemService, IIdentityService identityService, ILoggerFactory loggerFactory)
		{
			this.advertisementItemPhotosUploader = advertisementItemPhotosUploader;
			this.advertisementItemService = advertisementItemService;
			this.identityService = identityService;
			this.logger = loggerFactory.CreateLogger<AdvertisementItemController>();
		}

		[HttpPost]
		[Route("UploadFiles")]
		public async Task<IActionResult> UploadAdvertisementItemPhotos()
		{
			try
			{
				AdvertisementItemPhotosNames photosPaths = await this.advertisementItemPhotosUploader.SaveAdvertisementPhotos(Request.Form.Files);
				return Json(photosPaths);
			}
			catch (Exception exc)
			{
				this.logger.LogError("Wystąpił błąd podcza uploadu zdjęć ogłoszenia: " + exc);
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("CreateAdvertisementItem")]
		public IActionResult CreateAdvertisementItem([FromBody]NewAdvertisementItem newAdvertisementModel)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				this.advertisementItemService.CreateNewAdvertisementItem(newAdvertisementModel, userId);
				return Json("Ok");
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie tworzenia nowego ogłoszenia: " + exc);
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("GetAdvertisements")]
		public async Task<IActionResult> GetAdvertisements([FromBody]AdvertisementsSearchModel searchModel)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var advertisements = await this.advertisementItemService.GetAdvertisements(searchModel, userId);
				return Json(advertisements);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie pobierania ogłoszeń: " + exc);
				return null;
			}
		}

		[HttpPost]
		[Route("DeleteAdvertisement")]
		public IActionResult DeleteAdvertisement([FromBody]int advertisementId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				bool success = this.advertisementItemService.DeleteAdvertisement(advertisementId, userId);
				return Json(success);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie usuwania ogłoszenia: " + exc);
				return Json(false);
			}
		}

		[HttpPost]
		[Route("RestartAdvertisement")]
		public IActionResult RestartAdvertisement([FromBody]int advertisementId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				bool success = this.advertisementItemService.RestartAdvertisement(advertisementId, userId);
				return Json(success);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie restartowania ogłoszenia: " + exc);
				return Json(false);
			}
		}

		

		[HttpPost]
		[Route("DeleteAdvertisementFromFavourites")]
		public IActionResult DeleteAdvertisementFromFavourites([FromBody]int advertisementId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				bool success = this.advertisementItemService.DeleteAdvertisementFromFavourites(advertisementId, userId);
				return Json(success);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie usuwania ogłoszenia z ulubionych: " + exc);
				return Json(false);
			}
		}


		

		[HttpPost]
		[Route("CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck")]
		public IActionResult CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck([FromBody]CoordinatesForAdvertisementsModel coordinatesModel)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var foundedNewAdvertisements = this.advertisementItemService.CheckForNewAdvertisementsSinceLastCheck(userId, coordinatesModel, true);
				return Json(foundedNewAdvertisements);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie sprawdzania nowych ogłoszeń: " + exc);
				return null;
			}
		}


		[HttpPost]
		[Route("AddToUserFavourites")]
		public IActionResult AddToUserFavourites([FromBody]SingleIdModelForPostRequests advertisementId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var success = this.advertisementItemService.AddToUserFavourites(userId, advertisementId.Id);
				var messeage = success ? "Ogłoszenie zostało dodane do schowka." : "Ogłoszenie jest już w Twoim schowku";
				return Json(messeage);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie dodawania ogłoszenia do ulubionych: " + exc);
				return Json("Wystąpił błąd na serwerze. Spróbuj ponownie później");
			}
		}

		[HttpPost]
		[Route("CheckForNewAdvertisementsAroundHomeLocationSinceLastCheck")]
		public IActionResult CheckForNewAdvertisementsAroundHomeLocationSinceLastCheck([FromBody]CoordinatesForAdvertisementsModel coordinatesModel)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var foundedNewAdvertisements = this.advertisementItemService.CheckForNewAdvertisementsSinceLastCheck(userId, coordinatesModel, false);
				return Json(foundedNewAdvertisements);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie sprawdzania nowych ogłoszeń: " + exc);
				return null;
			}
		}

		[HttpGet]
		[Route("GetUserAdvertisements/{pageNumber}/{userId}")]
		public async Task<IActionResult> GetUserAdvertisements(int pageNumber, string userId)
		{
			try
			{
				var advertisements = await this.advertisementItemService.GetUserAdvertisements(userId, pageNumber);
				return Json(advertisements);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie pobierania ogłoszeń: " + exc);
				return null;
			}
		}

		[HttpGet]
		[Route("GetAdvertisementDetail/{advertisementId}")]
		public async Task<IActionResult> GetAdvertisementDetail(int advertisementId)
		{
			try
			{
				var userId = this.identityService.GetUserId(User.Identity);
				var advertisement = await this.advertisementItemService.GetAdvertisementDetails(advertisementId, userId);

				return Json(advertisement);
			}
			catch (Exception exc)
			{
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie pobierania szczegółów ofłoszenia: " + exc);
				return Json("Wystąpił błąd! - " + exc.Message);
			}
		}

	}
}
