using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Enumerations
{
	public class DisplayNameAttribute : Attribute
	{
		internal DisplayNameAttribute(string displayName)
		{
			this.DisplayName = displayName;
		}

		public string DisplayName { get; private set; }
	}
}
