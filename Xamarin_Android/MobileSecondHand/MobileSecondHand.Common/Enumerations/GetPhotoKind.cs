﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Common.Enumerations
{
	public enum GetPhotoKind
	{
		[DisplayName("Zrób zdjęcie")]
		TakeNewPhotoFromCamera = 1,
		[DisplayName("Wybierz istniejące zdjęcie")]
		TakeExistingPhotoFromStorage = 2
	}
}
