using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Services.Categories
{
	public interface ICategoryService
	{
		IDictionary<int, string> GetCategoriesForSettings();
	}
}
