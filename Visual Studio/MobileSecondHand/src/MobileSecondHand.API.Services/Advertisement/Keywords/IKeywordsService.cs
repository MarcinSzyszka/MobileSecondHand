using System.Collections.Generic;
using MobileSecondHand.DB.Models.Advertisement.Keywords;

namespace MobileSecondHand.API.Services.Advertisement.Keywords {
	public interface IKeywordsService
    {
		ICollection<T> RecognizeAndGetKeywordsDbModels<T>(string textToRecognize) where T : IKeywordDbModel;
		IEnumerable<string> RecognizeAndGetStringCollectionKeywords<T>(string textToRecognize);

	}
}
