using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNotNullOrEmpty(this string stringValue)
		{
			return stringValue != null && stringValue != String.Empty;
		}
	}
}
