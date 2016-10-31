using System.Collections.Generic;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.Services.Authentication
{
	public interface ISignInService
	{
		Task<bool> SignInUserWithBearerToken(TokenModel bearerToken);
		Task<TokenModel> SignInUserWithFacebookToken(FacebookTokenViewModel facebookToken);
		Task<TokenModel> SignInUserStandard(LoginModel loginModel);
		Task<TokenModel> RegisterUser(RegisterModel registerModel);
		Task<bool> SetUserName(string nickName, TokenModel bearerToken);
		Task<IEnumerable<UserInfoModel>> GetUserNamesModels(string bearerToken, string partName);
		Task<bool> UploadUserProfilePhoto(string bearerToken, byte[] photoByteArray);
		Task<byte[]> GetUserProfileImage(string bearerToken, string interlocutorId);
	}
}
