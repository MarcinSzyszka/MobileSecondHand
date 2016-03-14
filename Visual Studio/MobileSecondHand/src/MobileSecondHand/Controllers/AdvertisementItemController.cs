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

namespace MobileSecondHand.Controllers {
	[Route("api/[controller]")]
	public class AdvertisementItemController : Controller {
		IAdvertisementItemPhotosUploader advertisementItemPhotosUploader;
		IAdvertisementItemService advertisementItemService;

		public AdvertisementItemController(IAdvertisementItemPhotosUploader advertisementItemPhotosUploader, IAdvertisementItemService advertisementItemService) {
			this.advertisementItemPhotosUploader = advertisementItemPhotosUploader;
			this.advertisementItemService = advertisementItemService;
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
				this.advertisementItemService.CreateNewAdvertisementItem(newAdvertisementModel);
				return Json("Ok");
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}

		[HttpGet]
		[Authorize("Bearer")]
		[Route("GetAdvertisements")]
		public IEnumerable<AdvertisementItemShortModel> GetAdvertisements([FromBody]CoordinatesModel coordinatesModel) {
			try {
				var advertisements = this.advertisementItemService.GetAdvertisements(coordinatesModel);
				return advertisements;
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return null;
			}
		}
	}
}
