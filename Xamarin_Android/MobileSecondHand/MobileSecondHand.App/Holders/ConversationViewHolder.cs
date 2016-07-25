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

namespace MobileSecondHand.App.Holders
{
	public class ConversationViewHolder : RecyclerView.ViewHolder
	{
		public TextView InterlocutorNameTextView { get; set; }
		public TextView LastMessageTextView { get; set; }
		public TextView LastMessageDateTextView { get; set; }
		public ConversationViewHolder(View itemView, Action<int> clickAction) : base(itemView)
		{
			this.InterlocutorNameTextView = itemView.FindViewById<TextView>(Resource.Id.conversationInterlocutorNameTextView);
			this.LastMessageTextView = itemView.FindViewById<TextView>(Resource.Id.conversationLastMessageTextView);
			this.LastMessageDateTextView = itemView.FindViewById<TextView>(Resource.Id.conversationLastMessageDateTExtView);

			itemView.Click += (s, e) => clickAction(Position);
		}
	}
}