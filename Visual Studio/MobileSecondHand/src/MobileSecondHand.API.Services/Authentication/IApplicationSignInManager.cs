using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Security;
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.API.Services.Authentication
{
	public interface IApplicationSignInManager
	{
		Task<TokenModel> LoginWithFacebook(FacebookTokenViewModel facebookToken);
		Task<TokenModel> Register(RegisterModel registerViewModel);
		Task<TokenModel> LoginStandard(LoginModel loginStandardViewModel);
		Task<bool> IsUserNameSetByHimself(string userId);
		Task<bool> SetUserName(string userId, string nickName);
		IEnumerable<UserInfoModel> GetUserNamesModels(string userId, string partName);
	}
}
