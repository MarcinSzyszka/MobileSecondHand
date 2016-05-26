using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Services.Authentication
{
	public class IdentityService : IIdentityService {
		public string GetUserId(IIdentity identity) {
			var claimsIdentity = (ClaimsIdentity)identity;
			var userId = claimsIdentity.Claims.FirstOrDefault(c => c.Type.ToLower() == "userid").Value;

			return userId;
		}
	}
}
