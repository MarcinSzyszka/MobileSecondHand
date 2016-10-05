using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileSecondHand.API.Services.Categories;

namespace MobileSecondHand.Controllers
{
	[Produces("application/json")]
	[Route("api/Category")]
	[Microsoft.AspNetCore.Authorization.Authorize("Bearer")]
	public class CategoryController : Controller
	{
		ICategoryService categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			this.categoryService = categoryService;
		}

		[HttpGet("GetCategoriesForSettings")]
		public IDictionary<int, string> GetCategoriesForSettings()
		{
			var result = this.categoryService.GetCategoriesForSettings();

			return result;
		}
	}
}