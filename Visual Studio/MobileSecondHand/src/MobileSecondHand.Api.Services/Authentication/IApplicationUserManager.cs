using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MobileSecondHand.Db.Models;

namespace MobileSecondHand.Api.Services.Authentication
{
	public interface IApplicationUserManager {
		Task<ApplicationUser> GetUser(string email);
		Task<IdentityResult> CreateAsync(ApplicationUser user);
		Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo userLoginInfo);
		Task<bool> PasswordIsValid(ApplicationUser user, string password);
		Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
	}
}
