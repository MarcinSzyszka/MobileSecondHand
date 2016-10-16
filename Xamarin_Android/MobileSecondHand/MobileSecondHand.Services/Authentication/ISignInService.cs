using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.Services.Authentication
{
	public interface ISignInService {
		Task<bool> SignInUserWithBearerToken(TokenModel bearerToken);
		Task<TokenModel> SignInUserWithFacebookToken(FacebookTokenViewModel facebookToken);
		Task<TokenModel> SignInUserStandard(LoginModel loginModel);
		Task<TokenModel> RegisterUser(RegisterModel registerModel);
	}
}
