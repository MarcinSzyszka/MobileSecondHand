using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.API.Services.Authentication {
	public interface IApplicationUserManager {
		Task<ApplicationUser> GetUser(string email);
		Task<IdentityResult> CreateAsync(ApplicationUser user);
		Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo userLoginInfo);
		Task<bool> PasswordIsValid(ApplicationUser user, string password);
		Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
	}
}
