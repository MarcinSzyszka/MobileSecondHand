using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.API.Services.Authentication {
	public interface IApplicationUserManager {
		Task<ApplicationUser> GetUserByEmail(string email);
		Task<ApplicationUser> GetUserById(string userId);
		Task<IdentityResult> CreateAsync(ApplicationUser user);
		Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo userLoginInfo);
		Task<bool> PasswordIsValid(ApplicationUser user, string password);
		Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
		Task<ApplicationUser> GetByUserName(string nickName);
		Task<IdentityResult> SaveUserName(ApplicationUser user, string nickName);
		IQueryable<ApplicationUser> GetAllUsers();
		Task<IdentityResult> UpdateUserMdel(ApplicationUser user);
	}
}
