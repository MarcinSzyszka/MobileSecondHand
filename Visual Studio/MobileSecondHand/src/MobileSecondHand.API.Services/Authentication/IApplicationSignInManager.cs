using System.Threading.Tasks;
using MobileSecondHand.API.Models.Security;

namespace MobileSecondHand.API.Services.Authentication {
	public interface IApplicationSignInManager {
		Task<TokenModel> LoginWithFacebook(FacebookTokenViewModel facebookToken);
		Task<TokenModel> Register(RegisterViewModel registerViewModel);
		Task<TokenModel> LoginStandard(LoginViewModel loginStandardViewModel);
	}
}
