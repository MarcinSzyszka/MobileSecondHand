using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileSecondHand.API.Models.Advertisement;
using MobileSecondHand.API.Models.CustomResponsesModels;
using MobileSecondHand.API.Services.Advertisement;
using MobileSecondHand.API.Services.Authentication;
using System.Web.Script.Serialization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;

namespace MobileSecondHand.Controllers {
	[Microsoft.AspNetCore.Authorization.Authorize("Bearer")]
	[Route("api/[controller]")]
	public class AdvertisementItemController : Controller {
		IAdvertisementItemPhotosService advertisementItemPhotosUploader;
		IAdvertisementItemService advertisementItemService;
		IIdentityService identityService;
		private ILogger logger;

		public AdvertisementItemController(IAdvertisementItemPhotosService advertisementItemPhotosUploader, IAdvertisementItemService advertisementItemService, IIdentityService identityService, ILoggerFactory loggerFactory) {
			this.advertisementItemPhotosUploader = advertisementItemPhotosUploader;
			this.advertisementItemService = advertisementItemService;
			this.identityService = identityService;
			this.logger = loggerFactory.CreateLogger<AdvertisementItemController>(); ;
		}

		[HttpPost]
		[Route("UploadFiles")]
		public async Task<IActionResult> UploadAdvertisementItemPhotos() {
			try {
				AdvertisementItemPhotosPaths photosPaths = await this.advertisementItemPhotosUploader.SaveAdvertisementPhotos(Request.Form.Files);
				return Json(photosPaths);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("CreateAdvertisementItem")]
		public IActionResult CreateAdvertisementItem([FromBody]NewAdvertisementItemModel newAdvertisementModel) {
			try {
				logger.LogInformation("Tworzenia nowego ogłoszenia");
				var userId = this.identityService.GetUserId(User.Identity);
				this.advertisementItemService.CreateNewAdvertisementItem(newAdvertisementModel, userId);
				return Json("Ok");
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie tworzenia nowego ogłoszenia: " + exc.Message);
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpPost]
		[Route("GetAdvertisements")]
		public async Task<IActionResult> GetAdvertisements([FromBody]SearchAdvertisementsModel searchModel) {
			try {
				logger.LogInformation("Pobieranie ogłoszeń");
				var userId = this.identityService.GetUserId(User.Identity);
				var advertisements = await this.advertisementItemService.GetAdvertisements(searchModel, userId);
				logger.LogInformation("Zakonczono pobieranie ogłoszeń");
				return Json(advertisements);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie pobierania ogłoszeń: " + exc.Message);
				return null;
			}
		}

		[HttpGet]
		[Route("GetAdvertisementDetail/{advertisementId}")]
		public async Task<IActionResult> GetAdvertisementDetail(int advertisementId) {
			try {
				logger.LogInformation("Pobieranie szczegółu ogłoszenia");
				var userId = this.identityService.GetUserId(User.Identity);
				var advertisement = await this.advertisementItemService.GetAdvertisementDetails(advertisementId, userId);

				return Json(advertisement);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				logger.LogError("Wystąpił wyjątek w trakcie pobierania szczegółów ofłoszenia: " + exc.Message);
				return Json("Wystąpił błąd! - " + exc.Message);
			}
		}
		
	}
}
