using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Models.Enums;

namespace MobileSecondHand.Models.EventArgs
{
	public class FabOnAdvertisementItemRowClicked
	{
		public int Id { get; set; }
		public ActionKindAfterClickFabOnAdvertisementItemRow Action { get; set; }
		public bool IsExpired { get; set; }
	}
}
