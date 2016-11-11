using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Location;
using MobileSecondHand.Models.Consts;
using MobileSecondHand.Services.Factories;
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Advertisements
{
	public class AdvertisementItemService : IAdvertisementItemService
	{
		private HttpClient client;

		public AdvertisementItemService(string bearerToken)
		{
			this.client = HttpClientFactory.GetHttpClient(bearerToken);
		}
		public async Task<List<AdvertisementItemShort>> GetAdvertisements(AdvertisementsSearchModel searchModel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json");
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisements", stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return new List<AdvertisementItemShort>();
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}

		public async Task<List<AdvertisementItemShort>> GetUserAdvertisements(int pageNumber, string userId)
		{
			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetUserAdvertisements/" + pageNumber + "/" + userId);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return new List<AdvertisementItemShort>();
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}

		public async Task<bool> CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(AdvertisementsSearchModelForNotifications searchModel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json");
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck", stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var areTherNewAdvertisements = JsonConvert.DeserializeObject<bool>(responseContentString);
			return areTherNewAdvertisements;
		}

		public async Task<bool> CheckForNewAdvertisementsAroundHomeLocationSinceLastCheck(AdvertisementsSearchModelForNotifications searchModel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json");
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CheckForNewAdvertisementsAroundHomeLocationSinceLastCheck", stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var areTherNewAdvertisements = JsonConvert.DeserializeObject<bool>(responseContentString);
			return areTherNewAdvertisements;
		}

		public async Task<AdvertisementItemPhotosNames> UploadNewAdvertisementPhotos(IEnumerable<byte[]> bytesArrayList)
		{
			MultipartFormDataContent form = new MultipartFormDataContent();

			var sb = new StringBuilder();
			var i = 0;
			foreach (var byteArrayPhoto in bytesArrayList)
			{
				HttpContent content = new StringContent("uploadPhoto");
				sb.Append("photo");
				sb.Append(i);
				var stream = new MemoryStream(byteArrayPhoto);
				content = new StreamContent(stream);
				content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
				{
					Name = sb.ToString(),
					FileName = sb.ToString()
				};
				form.Add(content);
				i++;
				sb.Clear();
			}
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "UploadFiles", form);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();

			var photosPaths = JsonConvert.DeserializeObject<AdvertisementItemPhotosNames>(responseContentString);
			return photosPaths;
		}

		public async Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(newAdvertisementModel), Encoding.UTF8, "application/json");
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CreateAdvertisementItem", stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			return true;

		}

		public async Task<string> AddToUserFavouritesAdvertisements(SingleIdModelForPostRequests advertisementId)
		{
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "AddToUserFavourites", new StringContent(JsonConvert.SerializeObject(advertisementId), Encoding.UTF8, "application/json"));
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			var responseContentString = await response.Content.ReadAsStringAsync();

			return responseContentString.Replace("\"", "");
		}

		public async Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementItemId)
		{
			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisementDetail/" + advertisementItemId);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementDetails = JsonConvert.DeserializeObject<AdvertisementItemDetails>(responseContentString);

			return advertisementDetails;
		}

		public async Task<bool> RestartAdvertisement(int advertisementId)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(advertisementId), Encoding.UTF8, "application/json");
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "RestartAdvertisement", stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var success = JsonConvert.DeserializeObject<bool>(responseContentString);

			return success;
		}
		public async Task<bool> DeleteAdvertisement(int advertisementId, AdvertisementsKind advertisementsKind)
		{
			var actionaname = advertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser ? "DeleteAdvertisement/" : "DeleteAdvertisementFromFavourites/";

			var stringContent = new StringContent(JsonConvert.SerializeObject(advertisementId), Encoding.UTF8, "application/json");
			HttpResponseMessage response;
			try
			{
				response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + actionaname, stringContent);
			}
			catch
			{
				response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}

			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var success = JsonConvert.DeserializeObject<bool>(responseContentString);

			return success;
		}
	}
}
