using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Services.Chat;
using MobileSecondHand.App.Consts;
using System.Threading.Tasks;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.Models.Security;
using Newtonsoft.Json;
using Android.Views;
using System;

namespace MobileSecondHand.App.Activities
{
	[Activity]
	public class ConversationsListActivity : BaseActivity, IInfiniteScrollListener
	{
		IMessagesService messagesService;
		RecyclerView conversationsRecyclerView;
		ConversationsListAdapter conversationsListAdapter;
		private int conversationsPage;
		ProgressDialogHelper progress;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.messagesService = new MessagesService(bearerToken);
			SetContentView(Resource.Layout.ConversationsListActivity);
			base.SetupToolbar();
			conversationsPage = savedInstanceState == null ? 0 : savedInstanceState.GetInt(ExtrasKeys.CONVERSATIONS_LIST_PAGE);
			await SetupViews(savedInstanceState != null);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.conversationsListMenu, menu);
			if (menu != null)
			{
				menu.FindItem(Resource.Id.home).SetVisible(true);
			}

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var handled = false;
			switch (item.ItemId)
			{
				case Resource.Id.home:
					GoToMainPage();
					handled = true;
					break;
			}

			return handled;
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			SharedObject.Data = this.conversationsListAdapter.ConversationItems;
			outState.PutInt(ExtrasKeys.CONVERSATIONS_LIST_PAGE, conversationsPage);
			base.OnSaveInstanceState(outState);
		}

		private async Task SetupViews(bool screenOrientationChaged)
		{
			progress = new ProgressDialogHelper(this);
			this.conversationsRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			var mLayoutManager = new Android.Support.V7.Widget.LinearLayoutManager(this);
			this.conversationsRecyclerView.SetLayoutManager(mLayoutManager);
			await DownloadAndShowConversations(screenOrientationChaged ? false : true, screenOrientationChaged);
		}


		private async Task DownloadAndShowConversations(bool resetList, bool screenOrientationChaged = false)
		{
			progress.ShowProgressDialog("Pobieranie rozmów. Proszê czekaæ...");
			SetConversationsListPageNumber(resetList, screenOrientationChaged);
			List<ConversationItemModel> conversations = await GetStoredOrDownloadConversations(screenOrientationChaged);

			if (conversations != null && conversations.Count > 0)
			{
				if (conversationsListAdapter == null || resetList)
				{
					conversationsListAdapter = new ConversationsListAdapter(conversations, this);
					conversationsListAdapter.ConversationItemClick += ConversationsListAdapter_ConversationItemClick; ;
					var mLayoutManager = new LinearLayoutManager(this);
					conversationsRecyclerView.SetLayoutManager(mLayoutManager);
					conversationsRecyclerView.SetAdapter(conversationsListAdapter);
					conversationsRecyclerView.RequestLayout();
				}
				else
				{
					conversationsListAdapter.AddConversations(conversations);
				}
			}
			else
			{
				if (conversationsListAdapter == null)
				{
					conversationsListAdapter = new ConversationsListAdapter(new List<ConversationItemModel>(), this);
				}
				conversationsListAdapter.InfiniteScrollDisabled = true;
			}
			progress.CloseProgressDialog();
		}

		private void ConversationsListAdapter_ConversationItemClick(object sender, ConversationItemModel e)
		{
			var intent = new Intent(this, typeof(ConversationActivity));
			var conversationInfoModel = new ConversationInfoModel
			{
				ConversationId = e.Id,
				InterlocutorId = e.InterLocutorId,
				InterlocutorName = e.InterlocutorName
			};

			intent.PutExtra(ExtrasKeys.CONVERSATION_INFO_MODEL, JsonConvert.SerializeObject(conversationInfoModel));

			StartActivity(intent);
		}

		private async Task<List<ConversationItemModel>> GetStoredOrDownloadConversations(bool screenOrientationChaged)
		{
			List<ConversationItemModel> conversations;
			if (screenOrientationChaged)
			{
				conversations = (List<ConversationItemModel>)SharedObject.Data;
			}
			else
			{
				conversations = await GetConversations();
			}

			return conversations;
		}

		private async Task<List<ConversationItemModel>> GetConversations()
		{
			var list = await this.messagesService.GetConversations(conversationsPage);
			return list;
		}

		public async void OnInfiniteScroll()
		{
			await DownloadAndShowConversations(false);
		}


		private void SetConversationsListPageNumber(bool resetList, bool screenOrientationChaged)
		{
			if (resetList)
			{
				conversationsPage = 0;
			}
			else if (!screenOrientationChaged)
			{
				conversationsPage++;
			}
		}
	}
}