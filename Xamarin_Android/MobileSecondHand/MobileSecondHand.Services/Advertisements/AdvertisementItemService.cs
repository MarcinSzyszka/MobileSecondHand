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
		public async Task<List<AdvertisementItemShort>> GetAdvertisements(SearchModel searchModel, TokenModel tokenModel) {
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + tokenModel.Token);
			var stringContent = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisements", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}

		public async Task<AdvertisementItemPhotosPaths> UploadNewAdvertisementPhotos(byte[] bytesArray, TokenModel tokenModel) {
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + tokenModel.Token);

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

		public async Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel, TokenModel tokenModel) {
			var client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + tokenModel.Token);

			var stringContent = new StringContent(JsonConvert.SerializeObject(newAdvertisementModel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CreateAdvertisementItem", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK) {
				return false;
			}
			return true;

		}
	}
}
