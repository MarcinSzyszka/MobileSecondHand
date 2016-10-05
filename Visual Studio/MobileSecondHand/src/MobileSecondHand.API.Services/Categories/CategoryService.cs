using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Services.Advertisement.Categories;

namespace MobileSecondHand.API.Services.Categories
{
	public class CategoryService : ICategoryService
	{
		ICategoryDbService categoryDbService;

		public CategoryService(ICategoryDbService categoryDbService)
		{
			this.categoryDbService = categoryDbService;
		}

		public IDictionary<int, string> GetCategoriesForSettings()
		{
			var resultDic = new Dictionary<int, string>();

			var categories = this.categoryDbService.GetCategories().ToList();

			foreach (var category in categories)
			{
				resultDic.Add(category.Id, category.Name);
			}

			return resultDic;

		}
	}
}
