using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Db.Models.Advertisement.Keywords;

namespace MobileSecondHand.Db.Services.Advertisement.Keywords
{
    public interface IKeywordsDbService
    {
		IEnumerable<T> GetKeywordsByNames<T>(IEnumerable<string> keywordsNames) where T : IKeyword;
		void AddCategoryKeywordToContext(CategoryKeyword categoryKeyword);
		void AddColorKeywordToContext(ColorKeyword colorKeyword);
		void SaveChanges();
	}
}
