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
using Newtonsoft.Json;

namespace MobileSecondHand.Services.Advertisements
{
	public class AdvertisementItemService : IAdvertisementItemService
	{
		private HttpClient client;

		public AdvertisementItemService(string bearerToken)
		{
			this.client = new HttpClient();
			client.BaseAddress = new Uri(WebApiConsts.WEB_API_URL);
			client.DefaultRequestHeaders.Add(WebApiConsts.AUTHORIZATION_HEADER_NAME, WebApiConsts.AUTHORIZATION_HEADER_BEARER_VALUE_NAME + bearerToken);
		}
		public async Task<List<AdvertisementItemShort>> GetAdvertisements(AdvertisementsSearchModel searchModel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisements", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return new List<AdvertisementItemShort>();
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}

		public async Task<List<AdvertisementItemShort>> GetUserAdvertisements(int pageNumber)
		{
			var response = await client.GetAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetUserAdvertisements/" + pageNumber);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return new List<AdvertisementItemShort>();
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}

		public async Task<List<AdvertisementItemShort>> GetUserFavouritesAdvertisements(int pageNumber)
		{
			var response = await client.GetAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetUserFavouritesAdvertisements/" + pageNumber);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return new List<AdvertisementItemShort>();
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementList = JsonConvert.DeserializeObject<List<AdvertisementItemShort>>(responseContentString);
			return advertisementList;
		}

		public async Task<bool> CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck(CoordinatesForAdvertisementsModel coordinatesMOdel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(coordinatesMOdel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CheckForNewAdvertisementsAroundCurrentLocationSinceLastCheck", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var areTherNewAdvertisements = JsonConvert.DeserializeObject<bool>(responseContentString);
			return areTherNewAdvertisements;
		}

		public async Task<bool> CheckForNewAdvertisementsAroundHomeLocationSinceLastCheck(CoordinatesForAdvertisementsModel coordinatesMOdel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(coordinatesMOdel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CheckForNewAdvertisementsAroundHomeLocationSinceLastCheck", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var areTherNewAdvertisements = JsonConvert.DeserializeObject<bool>(responseContentString);
			return areTherNewAdvertisements;
		}

		public async Task<AdvertisementItemPhotosPaths> UploadNewAdvertisementPhotos(IEnumerable<byte[]> bytesArrayList)
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

			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "UploadFiles", form);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();

			var photosPaths = JsonConvert.DeserializeObject<AdvertisementItemPhotosPaths>(responseContentString);
			return photosPaths;
		}

		public async Task<bool> CreateNewAdvertisement(NewAdvertisementItem newAdvertisementModel)
		{
			var stringContent = new StringContent(JsonConvert.SerializeObject(newAdvertisementModel), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "CreateAdvertisementItem", stringContent);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return false;
			}
			return true;

		}

		public async Task<string> AddToUserFavouritesAdvertisements(SingleIdModelForPostRequests advertisementId)
		{
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "AddToUserFavourites", new StringContent(JsonConvert.SerializeObject(advertisementId), Encoding.UTF8, "application/json"));
			var responseContentString = await response.Content.ReadAsStringAsync();

			return responseContentString.Replace("\"", "");
		}

		public async Task<AdvertisementItemDetails> GetAdvertisementDetails(int advertisementItemId)
		{
			var response = await client.GetAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + "GetAdvertisementDetail/" + advertisementItemId);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				return null;
			}
			var responseContentString = await response.Content.ReadAsStringAsync();
			var advertisementDetails = JsonConvert.DeserializeObject<AdvertisementItemDetails>(responseContentString);

			return advertisementDetails;
		}

		public async Task<bool> DeleteAdvertisement(int advertisementId, AdvertisementsKind advertisementsKind)
		{
			var actionaname = advertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser ? "DeleteAdvertisement/" : "DeleteAdvertisementFromFavourites/";

			var stringContent = new StringContent(JsonConvert.SerializeObject(advertisementId), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(WebApiConsts.ADVERTISEMENT_CONTROLLER + actionaname, stringContent);
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
