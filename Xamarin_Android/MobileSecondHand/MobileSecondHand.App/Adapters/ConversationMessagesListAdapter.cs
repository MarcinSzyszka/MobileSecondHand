using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Holders;
using MobileSecondHand.Models.Chat;

namespace MobileSecondHand.App.Adapters {
	public class ConversationMessagesListAdapter : RecyclerView.Adapter {
		List<ConversationMessage> messages;
		public override int ItemCount { get { return this.messages.Count; } }

		public ConversationMessagesListAdapter(List<ConversationMessage> messages) {
			this.messages = messages;
		}

		public void AddMessage(ConversationMessage message) {
			this.messages.Add(message);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
			var currentItem = this.messages[position];
			ConversationMessageViewHolder vh = holder as ConversationMessageViewHolder;
			RelativeLayout.LayoutParams parameters = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
			parameters.AddRule(LayoutRules.AlignParentRight);
			vh.MessageLayout.LayoutParameters = parameters;
			vh.MessageLayout.SetBackgroundColor(currentItem.UserWasSender ? Color.SkyBlue : Color.LightGreen);
			vh.MessageHeader.Text = currentItem.MessageHeader;
			vh.MessageContent.Text = currentItem.MessageContent;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ConversationMessageRowView, parent, false);
			ConversationMessageViewHolder vh = new ConversationMessageViewHolder(itemView);
			return vh;
		}
	}
}