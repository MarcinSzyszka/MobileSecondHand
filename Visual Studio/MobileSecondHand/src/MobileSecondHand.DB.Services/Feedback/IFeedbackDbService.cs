using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models.Feedback;

namespace MobileSecondHand.DB.Services.Feedback
{
	public interface IFeedbackDbService
	{
		void SaveWrongAdvertisementIssue(WrongAdvertisementIssue issue);
	}
}
