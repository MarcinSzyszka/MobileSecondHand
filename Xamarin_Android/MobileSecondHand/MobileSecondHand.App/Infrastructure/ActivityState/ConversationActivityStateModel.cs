using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileSecondHand.App.Infrastructure.ActivityState
{
	public class ConversationActivityStateModel
	{
		public bool IsInForeground { get; }
		public int ConversationId { get; }

		public ConversationActivityStateModel(bool isInForeground, int conversationId)
		{
			this.IsInForeground = isInForeground;
			this.ConversationId = conversationId;
		}
	}
}