using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Security
{
	public class TokenModel
	{
		public string Token { get; set; }
		public bool UserHasToSetNickName { get; set; }
	}
}
