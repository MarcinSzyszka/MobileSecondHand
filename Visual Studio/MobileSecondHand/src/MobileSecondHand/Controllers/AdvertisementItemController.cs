using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Api.Models.Coordinates;
using MobileSecondHand.Api.Models.CustomResponsesModels;
using MobileSecondHand.Api.Services.Advertisement;
using MobileSecondHand.Api.Services.Authentication;

namespace MobileSecondHand.Controllers {
	[Authorize("Bearer")]
	[Route("api/[controller]")]
	public class AdvertisementItemController : Controller {
		IAdvertisementItemPhotosService advertisementItemPhotosUploader;
		IAdvertisementItemService advertisementItemService;
		IIdentityService identityService;

		public AdvertisementItemController(IAdvertisementItemPhotosService advertisementItemPhotosUploader, IAdvertisementItemService advertisementItemService, IIdentityService identityService) {
			this.advertisementItemPhotosUploader = advertisementItemPhotosUploader;
			this.advertisementItemService = advertisementItemService;
			this.identityService = identityService;
		}

		[HttpPost]
		[Route("UploadFiles")]
		public async Task<IActionResult> UploadAdvertisementItemPhotos() {
			try {
				AdvertisementItemPhotosPaths photosPaths = await advertisementItemPhotosUploader.SaveAdvertisementPhotos(Request.Form.Files);
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
				var userId = this.identityService.GetUserId(User.Identity);
				this.advertisementItemService.CreateNewAdvertisementItem(newAdvertisementModel, userId);
				return Json("Ok");
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpGet]
		[Route("GetAdvertisements")]
		public async Task<IEnumerable<AdvertisementItemShortModel>> GetAdvertisements([FromBody]CoordinatesModel coordinatesModel) {
			try {
				var userId = this.identityService.GetUserId(User.Identity);
				var advertisements = await this.advertisementItemService.GetAdvertisements(coordinatesModel, userId);
				return advertisements;
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return null;
			}
		}
	}
}
