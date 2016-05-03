using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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

		[HttpPost]
		[Route("GetAdvertisements")]
		public async Task<string> GetAdvertisements([FromBody]SearchModel searchModel) {
			try {
				var userId = this.identityService.GetUserId(User.Identity);
				var advertisements = await this.advertisementItemService.GetAdvertisements(searchModel, userId);
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				serializer.MaxJsonLength = int.MaxValue;
				var adverts = serializer.Serialize(advertisements);
					
				return adverts;
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return null;
			}
		}

		[HttpGet]
		[Route("GetAdvertisementDetail/{advertisementId}")]
		public async Task<IActionResult> GetAdvertisementDetail(int advertisementId) {
			try {
				var userId = this.identityService.GetUserId(User.Identity);
				var advertisement = await this.advertisementItemService.GetAdvertisementDetails(advertisementId, userId);

				return Json(advertisement);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json("Wystąpił błąd! - " + exc.Message);
			}
		}
		
	}
}
