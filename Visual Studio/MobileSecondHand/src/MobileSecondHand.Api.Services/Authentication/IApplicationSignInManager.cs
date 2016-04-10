using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileSecondHand.Api.Models.Security;

namespace MobileSecondHand.Api.Services.Authentication
{
	public interface IApplicationSignInManager {
		Task<TokenModel> LoginWithFacebook(FacebookTokenViewModel facebookToken);
		Task<TokenModel> Register(RegisterViewModel registerViewModel);
		Task<TokenModel> LoginStandard(LoginViewModel loginStandardViewModel);
	}
}
