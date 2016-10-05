using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Advertisement;

namespace MobileSecondHand.DB.Services.Advertisement.Categories
{
	public interface ICategoryDbService
	{
		IEnumerable<Category> GetCategories();
	}
}
