using System;
using System.Collections.Generic;
using System.Linq;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Keywords;

namespace MobileSecondHand.DB.Services.Advertisement.Keywords
{
	public class KeywordsDbService : IKeywordsDbService
    {
		MobileSecondHandContext dbContext;

		public KeywordsDbService(MobileSecondHandContext dbContext) {
			this.dbContext = dbContext;
		}

		public void AddCategoryKeywordToContext(CategoryKeyword categoryKeyword) {
			this.dbContext.CategoryKeyword.Add(categoryKeyword);
		}

		public void AddColorKeywordToContext(ColorKeyword colorKeyword) {
			this.dbContext.ColorKeyword.Add(colorKeyword);
		}

		public IEnumerable<CategoryKeyword> GetAllKeywords()
		{
			return dbContext.CategoryKeyword.ToList();
		}

		public IEnumerable<T> GetKeywordsByNames<T>(IEnumerable<string> keywordsNames) where T : IKeywordDbModel {
			if (typeof(T) == typeof(CategoryKeyword)) {
				var keywords = this.dbContext.CategoryKeyword.Where(k => keywordsNames.Contains(k.Name));
				return (IEnumerable <T>)keywords;
			} else if (typeof(T) == typeof(ColorKeyword)) {
				var keywords = this.dbContext.ColorKeyword.Where(k => keywordsNames.Contains(k.Name));
				return (IEnumerable<T>)keywords;
			}

			return new List<T>();
		}

		public void SaveChanges() {
			this.dbContext.SaveChanges();
		}
	}
}
