using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Models.Shared.Enumerations
{
	public enum AdvertisementsKind
	{
		[DisplayName("Ogłoszenia wokół Twojej aktualnej lokalizacji")]
		AdvertisementsAroundUserCurrentLocation = 1,
		[DisplayName("Ogłoszenia wokół Twojej domowej lokalizacji")]
		AdvertisementsArounUserHomeLocation = 2,
		[DisplayName("Twoje ogłoszenia")]
		AdvertisementsCreatedByUser = 3,
		[DisplayName("Schowek (zapamiętane)")]
		FavouritesAdvertisements = 4
	}
}
