using System.Collections.Generic;
using MobileSecondHand.DB.Models.Keywords;

namespace MobileSecondHand.API.Services.Keywords
{
	public interface IKeywordsService
    {
		ICollection<T> RecognizeAndGetKeywordsDbModels<T>(string textToRecognize) where T : IKeywordDbModel;
		IEnumerable<string> RecognizeAndGetStringCollectionKeywords<T>(string textToRecognize);
		IDictionary<int, string> GetKeywordsForSettings();

	}
}
