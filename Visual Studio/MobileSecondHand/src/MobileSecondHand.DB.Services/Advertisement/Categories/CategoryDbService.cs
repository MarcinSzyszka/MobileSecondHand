using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Services.Advertisement.Categories
{
	public class CategoryDbService : ICategoryDbService
	{
		private MobileSecondHandContext dbContext;

		public CategoryDbService(MobileSecondHandContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public IEnumerable<Category> GetCategories()
		{
			return this.dbContext.Category;
		}
	}
}
