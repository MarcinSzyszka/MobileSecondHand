using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MobileSecondHand.DB.Models")]
namespace MobileSecondHand.API.Models.Shared.Enumerations
{
	public enum ClothSize
	{
		[DisplayName("XS/34")]
		XS = 34,
		[DisplayName("S/36")]
		S = 36,
		[DisplayName("37")]
		SM = 37,
		[DisplayName("M/38")]
		M = 38,
		[DisplayName("39")]
		ML = 39,
		[DisplayName("L/40")]
		L = 40,
		[DisplayName("41")]
		LXL = 41,
		[DisplayName("XL/42")]
		XL = 42,
		[DisplayName("XXL/44")]
		XXL = 44,
		[DisplayName("XXXL/46")]
		XXXL = 46,
		[DisplayName("4XL/48")]
		FourXL = 48,
		[DisplayName("5XL/50")]
		FiveXL = 50,
		[DisplayName("inny")]
		Other = 100
	}
}
