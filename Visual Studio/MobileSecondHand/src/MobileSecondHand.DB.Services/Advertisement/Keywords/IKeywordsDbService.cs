using System.Collections.Generic;
using MobileSecondHand.DB.Models.Advertisement.Keywords;

namespace MobileSecondHand.DB.Services.Advertisement.Keywords {
	public interface IKeywordsDbService
    {
		IEnumerable<T> GetKeywordsByNames<T>(IEnumerable<string> keywordsNames) where T : IKeywordDbModel;
		void AddCategoryKeywordToContext(CategoryKeyword categoryKeyword);
		void AddColorKeywordToContext(ColorKeyword colorKeyword);
		void SaveChanges();
	}
}
