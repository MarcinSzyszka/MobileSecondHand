using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Api.Models.Security {
	public class TokenAuthorizationOptions {
		public string Audience { get; set; }
		public string Issuer { get; set; }
		public SigningCredentials SigningCredentials { get; set; }
	}
}
