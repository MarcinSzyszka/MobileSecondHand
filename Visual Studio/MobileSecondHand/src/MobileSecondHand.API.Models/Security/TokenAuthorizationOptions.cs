
using Microsoft.IdentityModel.Tokens;

namespace MobileSecondHand.API.Models.Security {
	public class TokenAuthorizationOptions {
		public string Audience { get; set; }
		public string Issuer { get; set; }
		public SigningCredentials SigningCredentials { get; set; }
	}
}
