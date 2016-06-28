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
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Chat;

namespace MobileSecondHand.App.Adapters {
	public class ConversationMessagesListAdapter : RecyclerView.Adapter {
		private IInfiniteScrollListener infiniteScrollListener;
		public override int ItemCount { get { return this.Messages.Count; } }
		public bool InfiniteScrollDisabled { get; set; }
		public List<ConversationMessage> Messages { get; private set; }

		public ConversationMessagesListAdapter(IInfiniteScrollListener infiniteScrollListener, List<ConversationMessage> messages) {
			this.infiniteScrollListener = infiniteScrollListener;
			this.Messages = messages;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
			var currentItem = this.Messages[position];
			ConversationMessageViewHolder vh = holder as ConversationMessageViewHolder;
			SetLayotParameters(currentItem, vh);
			vh.MessageHeader.Text = currentItem.MessageHeader;
			vh.MessageContent.Text = currentItem.MessageContent;

			RaiseOnInfiniteScrollWhenItemIsLastInList(currentItem, vh);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ConversationMessageRowView, parent, false);
			ConversationMessageViewHolder vh = new ConversationMessageViewHolder(itemView);
			return vh;
		}

		public void AddMessages(List<ConversationMessage> messages)
		{
			this.Messages.AddRange(messages);
			this.NotifyDataSetChanged();
		}

		public void AddReceivedMessage(ConversationMessage message)
		{
			this.Messages.Insert(0, message);
			this.NotifyDataSetChanged();
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

		private void RaiseOnInfiniteScrollWhenItemIsLastInList(ConversationMessage currentItem, ConversationMessageViewHolder viewHolder)
		{
			if (this.Messages.IndexOf(currentItem) == (this.Messages.Count - 1) && !InfiniteScrollDisabled)
			{
				this.infiniteScrollListener.OnInfiniteScroll();
			}
		}

	}
}