using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileSecondHand.API.Services.Keywords;

namespace MobileSecondHand.Controllers
{
    [Produces("application/json")]
    [Route("api/Keywords")]
    public class KeywordsController : Controller
    {
		IKeywordsService keywordsService;

		public KeywordsController(IKeywordsService keywordsService)
		{
			this.keywordsService = keywordsService;
		}

		[HttpGet("GetKeywordsForSettings")]
		public IDictionary<int, string> GetKeywordsForSettings()
		{
			var result = this.keywordsService.GetKeywordsForSettings();

			return result;
		}
	}
}