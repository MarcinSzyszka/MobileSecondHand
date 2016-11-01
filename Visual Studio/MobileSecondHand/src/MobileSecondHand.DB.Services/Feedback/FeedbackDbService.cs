using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.DB.Models;
using MobileSecondHand.DB.Models.Feedback;

namespace MobileSecondHand.DB.Services.Feedback
{
	public class FeedbackDbService : IFeedbackDbService
	{
		MobileSecondHandContext context;

		public FeedbackDbService(MobileSecondHandContext context)
		{
			this.context = context;
		}

		public void SaveWrongAdvertisementIssue(WrongAdvertisementIssue issue)
		{
			if (issue.Id > 0)
			{
				context.Entry(issue).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			}
			else
			{
				context.WrongAdvertisementIssue.Add(issue);
			}

			context.SaveChanges();
		}
	}
}
