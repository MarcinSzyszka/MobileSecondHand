using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Advertisement;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Models.Security;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Advertisements {
	public class AdvertisementItemService : IAdvertisementItemService {
		private HttpClient client;

		public AdvertisementItemService(string bearerToken)
		{
			this.client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
		}
		public async Task<List<AdvertisementItemShort>> GetAdvertisements(SearchAdvertisementsModel searchModel) {
		
			var stringContent = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisements", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}

		public async Task<AdvertisementItemPhotosPaths> UploadNewAdvertisementPhotos(byte[] bytesArray) {
			MultipartFormDataContent form = new MultipartFormDataContent();
			HttpContent content = new StringContent("uploadPhoto");
			form.Add(content, "uploadPhoto");
			var stream = new MemoryStream(bytesArray);
			content = new StreamContent(stream);
			content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") {
				Name = "photo0",
				FileName = "photo0"
			};
			form.Add(content);
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "UploadFiles" , form);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();

			var photosPaths = JsonConvert.DeserializeObject<AdvertisementItemPhotosPaths>(responseContentString);
			return photosPaths;
		}

		public async Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel) {
			var stringContent = new StringContent(JsonConvert.SerializeObject(newAdvertisementModel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CreateAdvertisementItem", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return false;
			}
			return true;

		}

		public async Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementItemId) {
			var response = await client.GetAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisementDetail/" + advertisementItemId);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementDetails = JsonConvert.DeserializeObject<AdvertisementItemDetails>(responseContentString);

			return advertisementDetails;
		}
	}
}
