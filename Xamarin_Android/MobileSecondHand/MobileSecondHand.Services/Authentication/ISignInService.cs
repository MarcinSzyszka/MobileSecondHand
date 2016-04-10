using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Security;

namespace MobileSecondHand.Services.Authentication {
	public interface ISignInService {
		Task<bool> SignInUserWithBearerToken(TokenModel bearerToken);
		Task<TokenModel> SignInUserWithFacebookToken(FacebookTokenViewModel facebookToken);
		Task<TokenModel> SignInUserStandard(LoginModel loginModel);
		Task<TokenModel> RegisterUser(RegisterModel registerModel);
	}
}
