using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Authentication;

namespace MobileSecondHand.COMMON.Extensions {
	public static class ApplictionUserExtensions {
		public static string GetUserName(this ApplicationUser user) {
			var atIdx = user.UserName.IndexOf('@');
			if (atIdx > -1) {
				return user.UserName.Substring(0, atIdx);
			}

			return user.UserName;
		}
	}
}
