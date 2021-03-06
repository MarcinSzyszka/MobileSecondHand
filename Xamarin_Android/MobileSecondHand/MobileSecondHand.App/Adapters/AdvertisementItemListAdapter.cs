using System;
using System.Collections.Generic;

using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using MobileSecondHand.API.Models.Shared.Advertisements;
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.App.Holders;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.EventArgs;
using MobileSecondHand.API.Models.Shared.Extensions;

namespace MobileSecondHand.App.Adapters
{
	public class AdvertisementItemListAdapter : RecyclerView.Adapter
	{
		Activity context;
		BitmapOperationService bitmapOperationService;
		IInfiniteScrollListener infiniteScrollListener;
		private AdvertisementsKind advertisementsKind;
		private int createdCount;

		public event EventHandler<ShowAdvertisementDetailsEventArgs> AdvertisementItemClick;
		public event EventHandler<FabOnAdvertisementItemRowClicked> DeleteAdvertisementItemClick;
		public event EventHandler<FabOnAdvertisementItemRowClicked> EditAdvertisementItemClick;

		public bool InfiniteScrollDisabled { get; set; }
		public List<AdvertisementItemShort> AdvertisementItems { get; private set; }

		public AdvertisementItemListAdapter(Activity context, List<AdvertisementItemShort> advertisementItems, AdvertisementsKind advertisementsKind, IInfiniteScrollListener infiniteScrollListener)
		{
			createdCount = 0;
			this.AdvertisementItems = advertisementItems;
			this.context = context;
			this.bitmapOperationService = new BitmapOperationService();
			this.infiniteScrollListener = infiniteScrollListener;
			this.advertisementsKind = advertisementsKind;
		}


		public override int ItemCount
		{
			get { return this.AdvertisementItems.Count; }
		}

		public void AddAdvertisements(List<AdvertisementItemShort> advertisements)
		{
			this.AdvertisementItems.AddRange(advertisements);
			this.NotifyDataSetChanged();
		}

		public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{

			var currentItem = this.AdvertisementItems[position];
			AdvertisementItemViewHolder vh = holder as AdvertisementItemViewHolder;
			if (advertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser || advertisementsKind == AdvertisementsKind.FavouritesAdvertisements)
			{
				vh.DeleteAdvertisementFab.Visibility = ViewStates.Visible;
				if (advertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser)
				{
					if (currentItem.IsExpired)
					{
						vh.DeleteAdvertisementFab.SetImageResource(Resource.Drawable.restart);
						vh.EditAdvertisementFab.Visibility = ViewStates.Invisible;
					}
					else
					{
						vh.EditAdvertisementFab.Visibility = ViewStates.Visible;
						vh.EditAdvertisementFab.BringToFront();
					}
				}
			}
			else
			{
				vh.EditAdvertisementFab.Visibility = ViewStates.Invisible;
				vh.DeleteAdvertisementFab.Visibility = ViewStates.Invisible;
			}

			if (currentItem.IsSellerOnline)
			{
				vh.SellerChatStateImageView.SetBackgroundResource(Resource.Drawable.rounded_chat_state_online);
			}
			else
			{
				vh.SellerChatStateImageView.SetBackgroundResource(Resource.Drawable.rounded_chat_state_offline);
			}
			if (currentItem.IsOnlyForSell)
			{
				vh.AdvertisementKindTextView.Text = "tylko sprzeda�";
			}
			vh.SizeTextView.Text = $"Rozmiar: {currentItem.Size.GetDisplayName()}";
			vh.DistanceTextView.Text = String.Format("{0} km", currentItem.Distance);
			vh.TitleTextView.Text = currentItem.AdvertisementTitle;
			vh.PriceTextView.Text = String.Format("{0} z�", currentItem.AdvertisementPrice);
			vh.PhotoImageView.SetImageBitmap(await bitmapOperationService.GetScaledDownBitmapForDisplayAsync(currentItem.MainPhoto));
			RaiseOnInfiniteScrollWhenItemIsLastInList(currentItem, vh);
			if (createdCount % 60 == 0)
			{
				//clean bitmaps after 60 attaches to avoid OOM Exception
				GC.Collect();
			}
			createdCount++;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AdvertisementItemRowView, parent, false);
			AdvertisementItemViewHolder vh = new AdvertisementItemViewHolder(itemView, OnAdvertisementItemClick, OnDeleteAdvertisementClick, OnEditAdvertisementClick);
			return vh;
		}

		private void OnDeleteAdvertisementClick(int positionId)
		{
			if (DeleteAdvertisementItemClick != null)
			{
				var clickArgs = new FabOnAdvertisementItemRowClicked();
				clickArgs.Id = this.AdvertisementItems[positionId].Id;
				if (this.AdvertisementItems[positionId].IsExpired && advertisementsKind == AdvertisementsKind.AdvertisementsCreatedByUser)
				{
					clickArgs.Action = Models.Enums.ActionKindAfterClickFabOnAdvertisementItemRow.Restart;
				}
				else if (this.AdvertisementItems[positionId].IsExpired || advertisementsKind == AdvertisementsKind.FavouritesAdvertisements)
				{
					clickArgs.Action = Models.Enums.ActionKindAfterClickFabOnAdvertisementItemRow.DeleteFromFavourites;
				}
				else
				{
					clickArgs.Action = Models.Enums.ActionKindAfterClickFabOnAdvertisementItemRow.MarkAsExpired;
				}
				DeleteAdvertisementItemClick(this, clickArgs);
			}
		}

		private void OnEditAdvertisementClick(int positionId)
		{
			if (EditAdvertisementItemClick != null)
			{
				var clickArgs = new FabOnAdvertisementItemRowClicked();
				clickArgs.Id = this.AdvertisementItems[positionId].Id;
				clickArgs.Action = Models.Enums.ActionKindAfterClickFabOnAdvertisementItemRow.Edit;

				EditAdvertisementItemClick(this, clickArgs);
			}
		}

		private void OnAdvertisementItemClick(int positionId)
		{
			if (AdvertisementItemClick != null)
			{
				AdvertisementItemClick(this, new ShowAdvertisementDetailsEventArgs { Id = this.AdvertisementItems[positionId].Id, Distance = this.AdvertisementItems[positionId].Distance });
			}
		}

		private void RaiseOnInfiniteScrollWhenItemIsLastInList(AdvertisementItemShort currentItem, AdvertisementItemViewHolder viewHolder)
		{
			if (this.AdvertisementItems.IndexOf(currentItem) == (this.AdvertisementItems.Count - 1) && !InfiniteScrollDisabled)
			{
				this.infiniteScrollListener.OnInfiniteScroll();
			}
		}

	}
}