using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Models.Consts {
	public class WebApiConsts {
#if DEBUG
		public const string SERWER_URL = "http://192.168.8.101:81/";
		public const string WEB_API_URL = "http://192.168.8.101:81/api/";
#else
		public const string SERWER_URL = "http://www.mobilesecondhand.pl/";
		public const string WEB_API_URL = "http://www.mobilesecondhand.pl/api/";
#endif

		public const string WEB_API_ACCOUNT_CONTROLLER = "WebApiAccount/";
		public const string ADVERTISEMENT_CONTROLLER = "AdvertisementItem/";
		public const string CONVERSATION_CONTROLLER = "Conversation";
		public const string KEYWORDS_CONTROLLER = "Keywords/";
		public const string CATEGORY_CONTROLLER = "Category/";
		public const string FEEDBACK_CONTROLLER = "Feedback/";

		public const string AUTHORIZATION_HEADER_NAME = "Authorization";
		public const string AUTHORIZATION_HEADER_BEARER_VALUE_NAME = "Bearer ";
	}
}
