using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Models.Shared.Enumerations;

namespace MobileSecondHand.Models.Enums
{
	public enum ActionKindAfterClickFabOnAdvertisementItemRow
	{
		[DisplayName("Czy na pewno chcesz zakończyć to ogłoszenie?")]
		MarkAsExpired = 1,
		[DisplayName("Czy na pewno chcesz usunąć z ulubionych to ogłoszenie?")]
		DeleteFromFavourites = 2,
		[DisplayName("Czy na pewno chcesz aktywować to ogłoszenie?")]
		Restart = 3
	}
}
