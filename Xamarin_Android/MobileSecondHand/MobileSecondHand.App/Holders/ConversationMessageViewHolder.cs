using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MobileSecondHand.App.Holders {
	public class ConversationMessageViewHolder : RecyclerView.ViewHolder {
		public RelativeLayout MessageLayout { get; set; }
		public TextView MessageHeader { get; set; }
		public TextView MessageContent { get; set; }
		public ConversationMessageViewHolder(View itemView) : base(itemView) {
			MessageHeader = itemView.FindViewById<TextView>(Resource.Id.conversationMessageHeader);
			MessageContent = itemView.FindViewById<TextView>(Resource.Id.conversationMessageContent);
			MessageLayout = itemView.FindViewById<RelativeLayout>(Resource.Id.relativeLayoutConversation);
		}
	}
}