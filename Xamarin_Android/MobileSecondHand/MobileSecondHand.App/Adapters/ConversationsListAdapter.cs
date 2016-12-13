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
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.App.Holders;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.API.Models.Shared.Chat;

namespace MobileSecondHand.App.Adapters
{
	public class ConversationsListAdapter : RecyclerView.Adapter
	{
		IInfiniteScrollListener infiniteScrollListener;
		BitmapOperationService bitmapService;
		public List<ConversationItemModel> ConversationItems { get; private set; }
		public event EventHandler<ConversationItemModel> ConversationItemClick;
		public bool InfiniteScrollDisabled { get; set; }
		public override int ItemCount
		{
			get { return this.ConversationItems.Count; }
		}

		public ConversationsListAdapter(List<ConversationItemModel> conversationsItems, IInfiniteScrollListener infiniteScrollListener)
		{
			this.ConversationItems = conversationsItems;
			this.infiniteScrollListener = infiniteScrollListener;
			this.bitmapService = new BitmapOperationService();
		}

		public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var currentItem = this.ConversationItems[position];
			ConversationViewHolder vh = holder as ConversationViewHolder;

			vh.InterlocutorNameTextView.Text = currentItem.InterlocutorName;
			vh.LastMessageTextView.Text = currentItem.LastMessage;
			vh.LastMessageDateTextView.Text = currentItem.LastMessageDate;
			if (currentItem.InterLocutorProfileImage != null)
			{
				vh.InterlocutorProfileImage.SetImageBitmap(await this.bitmapService.GetScaledDownBitmapForDisplayAsync(currentItem.InterLocutorProfileImage));
			}
			else
			{
				vh.InterlocutorProfileImage.SetImageResource(Resource.Drawable.logo_user);

			}
			vh.LastMessageDateTextView.Text = currentItem.LastMessageDate;

			RaiseOnInfiniteScrollWhenItemIsLastInList(currentItem, vh);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ConversationItemRovView, parent, false);
			ConversationViewHolder vh = new ConversationViewHolder(itemView, OnConversationItemClick);
			return vh;
		}

		private void OnConversationItemClick(int position)
		{
			if (ConversationItemClick != null)
			{
				ConversationItemClick(this, this.ConversationItems[position]);
			}
		}

		private void RaiseOnInfiniteScrollWhenItemIsLastInList(ConversationItemModel currentItem, ConversationViewHolder viewHolder)
		{
			if (this.ConversationItems.IndexOf(currentItem) == (this.ConversationItems.Count - 1) && !InfiniteScrollDisabled)
			{
				this.infiniteScrollListener.OnInfiniteScroll();
			}
		}

		public void AddConversations(List<ConversationItemModel> conversations)
		{
			this.ConversationItems.AddRange(conversations);
			this.NotifyDataSetChanged();
		}
	}
}