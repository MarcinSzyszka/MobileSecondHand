using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Services.Feedback
{
	public interface IFeedbackService
	{
		Task<bool> ReportWrongAdvertisement(int advertId, string reason);
	}
}
