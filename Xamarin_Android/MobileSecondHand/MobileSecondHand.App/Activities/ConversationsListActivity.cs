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
using Newtonsoft.Json;
using Android.Views;
using Android.Widget;
using MobileSecondHand.API.Models.Shared.Chat;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Rozmowy", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class ConversationsListActivity : BaseActivity, IInfiniteScrollListener
	{
		IMessagesService messagesService;
		RecyclerView conversationsRecyclerView;
		ConversationsListAdapter conversationsListAdapter;
		private int conversationsPage;
		ProgressDialogHelper progress;
		private TextView textViewNoConversations;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.messagesService = new MessagesService(bearerToken);
			SetContentView(Resource.Layout.ConversationsListActivity);
			base.SetupToolbar();
			conversationsPage = 0;
			await SetupViews();
		}

		//public override bool OnCreateOptionsMenu(IMenu menu)
		//{
		//	MenuInflater.Inflate(Resource.Menu.mainActivityMenu, menu);
		//	if (menu != null)
		//	{
		//		menu.FindItem(Resource.Id.applyFilterOptions).SetVisible(false);
		//		menu.FindItem(Resource.Id.clearFilterOptions).SetVisible(false);
		//		menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(false);
		//		menu.FindItem(Resource.Id.chat).SetVisible(false);
		//		menu.FindItem(Resource.Id.choosingAdvertisementsList).SetVisible(false);
		//		menu.FindItem(Resource.Id.home).SetVisible(true);
		//	}

		//	return base.OnCreateOptionsMenu(menu);
		//}

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

		private async Task SetupViews()
		{
			progress = new ProgressDialogHelper(this);
			this.textViewNoConversations = FindViewById<TextView>(Resource.Id.textViewNoConversations);
			this.conversationsRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			var mLayoutManager = new Android.Support.V7.Widget.LinearLayoutManager(this);
			this.conversationsRecyclerView.SetLayoutManager(mLayoutManager);
			await DownloadAndShowConversations(true);
		}


		private async Task DownloadAndShowConversations(bool resetList)
		{
			progress.ShowProgressDialog("Pobieranie rozmów. Proszê czekaæ...");
			SetConversationsListPageNumber(resetList);
			List<ConversationItemModel> conversations = await GetConversations();

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
				if (conversationsListAdapter.ItemCount == 0)
				{
					this.conversationsRecyclerView.Visibility = ViewStates.Gone;
					this.textViewNoConversations.Visibility = ViewStates.Visible;
				}
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
				InterlocutorName = e.InterlocutorName,
				InterlocutorPrifileImage = e.InterLocutorProfileImage
			};

			intent.PutExtra(ExtrasKeys.CONVERSATION_INFO_MODEL, JsonConvert.SerializeObject(conversationInfoModel));

			StartActivity(intent);
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


		private void SetConversationsListPageNumber(bool resetList)
		{
			if (resetList)
			{
				conversationsPage = 0;
			}
			else
			{
				conversationsPage++;
			}
		}
	}
}