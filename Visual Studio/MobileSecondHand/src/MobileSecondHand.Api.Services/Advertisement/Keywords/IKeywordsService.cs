using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Db.Models.Advertisement.Keywords;

namespace MobileSecondHand.Api.Services.Advertisement.Keywords
{
    public interface IKeywordsService
    {
		ICollection<T> RecognizeAndGetKeywordsDbModels<T>(string textToRecognize) where T : IKeywordDbModel;
		IEnumerable<string> RecognizeAndGetStringCollectionKeywords<T>(string textToRecognize);

	}
}
