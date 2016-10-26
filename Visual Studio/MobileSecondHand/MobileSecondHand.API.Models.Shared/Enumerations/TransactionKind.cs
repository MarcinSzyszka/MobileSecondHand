using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Enumerations
{
	public enum TransactionKind
	{
		[DisplayName("Wszystkie")]
		All = 1,
		[DisplayName("Tylko sprzedaż")]
		OnlyWithSell = 2,
		[DisplayName("Tylko z wymianą")]
		OnlyWithChange = 3
	}
}
