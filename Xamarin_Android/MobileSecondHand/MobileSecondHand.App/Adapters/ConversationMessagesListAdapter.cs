using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
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
			SetLayotParameters(currentItem, vh);
			vh.MessageHeader.Text = currentItem.MessageHeader;
			vh.MessageContent.Text = currentItem.MessageContent;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ConversationMessageRowView, parent, false);
			ConversationMessageViewHolder vh = new ConversationMessageViewHolder(itemView);
			return vh;
		}
		private static void SetLayotParameters(ConversationMessage currentItem, ConversationMessageViewHolder vh) {
			RelativeLayout.LayoutParams parameters = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
			if (currentItem.UserWasSender) {
				parameters.AddRule(LayoutRules.AlignParentLeft);
				parameters.SetMargins(5, 5, 30, 5);
				vh.MessageLayout.Background = ContextCompat.GetDrawable(Application.Context, Resource.Drawable.conversation_user_message_background_border);
				vh.MessageHeader.SetTextColor(Color.Black);
				vh.MessageContent.SetTextColor(Color.Black);
			}
			else {
				parameters.AddRule(LayoutRules.AlignParentRight);
				parameters.SetMargins(30, 5, 5, 5);
				vh.MessageLayout.Background = ContextCompat.GetDrawable(Application.Context, Resource.Drawable.conversation_sender_message_background_border);
				vh.MessageHeader.SetTextColor(Color.White);
				vh.MessageContent.SetTextColor(Color.White);
			}
			vh.MessageLayout.LayoutParameters = parameters;
		}

	}
}