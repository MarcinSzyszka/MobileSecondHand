﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Db.Models.Advertisement.Keywords;

namespace MobileSecondHand.Db.Services.Advertisement.Keywords
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

		public IEnumerable<T> GetKeywordsByNames<T>(IEnumerable<string> keywordsNames) where T : IKeyword {
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
