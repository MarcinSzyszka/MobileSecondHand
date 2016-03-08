using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MobileSecondHand.Api.Models.Advertisement;
using MobileSecondHand.Api.Models.CustomResponsesModels;
using MobileSecondHand.Api.Services.Advertisement;

namespace MobileSecondHand.Controllers
{
    [Route("api/[controller]")]
    public class AdvertisementItemController : Controller
    {
		IAdvertisementItemPhotosUploader advertisementItemPhotosUploader;

		public AdvertisementItemController(IAdvertisementItemPhotosUploader advertisementItemPhotosUploader) {
			this.advertisementItemPhotosUploader = advertisementItemPhotosUploader;
		}

		[HttpPost]
		[Route("UploadFiles")]
		public async Task<IActionResult> UploadFiles() {
			try {
				AdvertisementItemPhotosPaths photosPaths = await advertisementItemPhotosUploader.SaveAdvertisementPhotos(Request.Form.Files);
				return Json(photosPaths);
			} catch (Exception exc) {
				Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				return Json(new ErrorResponse { ErrorMessage = exc.Message });
			}
		}
    }
}
