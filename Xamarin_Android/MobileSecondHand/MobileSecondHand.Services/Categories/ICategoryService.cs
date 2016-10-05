using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Services.Categories
{
	public interface ICategoryService
	{
		Task<IDictionary<int, string>> GetCategories();
	}
}
