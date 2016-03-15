using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MobileSecondHand.Api.Services.Authentication
{
    public interface IIdentityService
    {
		string GetUserId(IIdentity identity);
    }
}
