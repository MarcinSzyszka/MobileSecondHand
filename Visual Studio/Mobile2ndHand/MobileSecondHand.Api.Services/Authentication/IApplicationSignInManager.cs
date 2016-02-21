using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Security;

namespace MobileSecondHand.Api.Services.Authentication
{
	public interface IApplicationSignInManager {
		Task<string> LoginWithFacebook(string facebookToken);
		Task<string> Register(RegisterViewModel registerViewModel);
		Task<string> LoginStandard(LoginViewModel loginStandardViewModel);
	}
}
