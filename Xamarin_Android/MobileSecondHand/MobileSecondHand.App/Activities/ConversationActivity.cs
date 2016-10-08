using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.Services.Chat;
using MobileSecondHand.App.Infrastructure.ActivityState;
using Newtonsoft.Json;
using MobileSecondHand.API.Models.Shared.Extensions;

namespace MobileSecondHand.App.Activities
{
	[Activity]
	public class ConversationActivity : BaseActivity, IInfiniteScrollListener
	{
		IMessagesService messagesService;
		ConversationMessagesListAdapter conversationMessagesListAdapter;
		ChatHubClientService chatHubClientService;
		RecyclerView conversationMessagesRecyclerView;
		ImageButton btnSendMessage;
		EditText editTextMessage;
		ConversationInfoModel conversationInfoModel;
		int pageNumber;

		public static ConversationActivityStateModel ConversationActivityStateModel { get; private set; } = new ConversationActivityStateModel(false, 0);
		public static ConversationActivity ActivityInstance { get; private set; }

		public async void OnInfiniteScroll()
		{
			await GetAndSetMessages();
		}

		public void AddMessage(ConversationMessage message)
		{
			this.RunOnUiThread(() => this.conversationMessagesListAdapter.AddReceivedMessage(message));
		}

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ActivityInstance = this;
			this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
			this.messagesService = new MessagesService(bearerToken);
			SetContentView(Resource.Layout.ConversationActivity);
			GetExtras();
			base.SetupToolbar();

			pageNumber = 0;
			await SetupViews(savedInstanceState);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.conversationMenu, menu);
			if (menu != null)
			{
				menu.FindItem(Resource.Id.home).SetVisible(true);
				menu.FindItem(Resource.Id.chat).SetVisible(true);
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
				case Resource.Id.chat:
					GoToChat();
					handled = true;
					break;
			}

			return handled;
		}


		public override bool DispatchKeyEvent(KeyEvent e)
		{
			if (e.KeyCode == Keycode.Back)
			{
				if (this.editTextMessage.HasFocus)
				{
					this.editTextMessage.ClearFocus();
				}
			}
			return base.DispatchKeyEvent(e);
		}

		protected override void OnStart()
		{
			base.OnStart();
			ConversationActivity.ConversationActivityStateModel = GetStateModel(true);
		}

		protected override void OnStop()
		{
			base.OnStop();
			ConversationActivity.ConversationActivityStateModel = GetStateModel(false);
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			SharedObject.Data = this.conversationMessagesListAdapter.Messages;
		}

		private ConversationActivityStateModel GetStateModel(bool isInForeground)
		{
			return new ConversationActivityStateModel(isInForeground, this.conversationInfoModel.ConversationId);
		}

		private void GetExtras()
		{
			var conversationInfoModelString = Intent.GetStringExtra(ExtrasKeys.CONVERSATION_INFO_MODEL);
			this.conversationInfoModel = JsonConvert.DeserializeObject<ConversationInfoModel>(conversationInfoModelString);
		}

		private async Task SetupViews(Bundle savedInstanceState)
		{
			conversationMessagesRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			btnSendMessage = FindViewById<ImageButton>(Resource.Id.buttonSendConversationMessage);
			btnSendMessage.Click += BtnSendMessage_Click;
			editTextMessage = FindViewById<EditText>(Resource.Id.editTextConversationMessage);
			editTextMessage.KeyPress += EditTextMessage_KeyPress;
			var mLayoutManager = new LinearLayoutManager(this);
			mLayoutManager.ReverseLayout = true;
			mLayoutManager.SmoothScrollbarEnabled = true;
			conversationMessagesRecyclerView.SetLayoutManager(mLayoutManager);

			this.conversationMessagesListAdapter = new ConversationMessagesListAdapter(this);
			this.conversationMessagesListAdapter.NewMessageAdded += ConversationMessagesListAdapter_NewMessageAdded;
			await GetAndSetMessages(savedInstanceState);
			this.conversationMessagesRecyclerView.SetAdapter(conversationMessagesListAdapter);
			this.conversationMessagesRecyclerView.RequestLayout();
		}

		private void ConversationMessagesListAdapter_NewMessageAdded(object sender, EventArgs e)
		{
			this.conversationMessagesRecyclerView.SmoothScrollToPosition(0);
		}

		private void EditTextMessage_KeyPress(object sender, View.KeyEventArgs e)
		{
			if (editTextMessage.Text.IsNotNullOrEmpty())
			{
				if (e.KeyCode == Keycode.DpadCenter || e.KeyCode == Keycode.Enter)
				{
					SendMessage();
				}
			}

		}

		private void BtnSendMessage_Click(object sender, EventArgs e)
		{
			SendMessage();
		}

		private void SendMessage()
		{
			if (editTextMessage.Text != null & editTextMessage.Text != string.Empty)
			{
				if (chatHubClientService.IsConnected())
				{
					var date = DateTime.Now;
					var message = new ConversationMessage();
					message.MessageContent = editTextMessage.Text;
					message.UserWasSender = true;
					message.ConversationId = this.conversationInfoModel.ConversationId;
					message.MessageHeader = String.Format("ja, {0} {1}", date.GetDateDottedStringFormat(), date.GetTimeColonStringFormat());

					this.conversationMessagesListAdapter.AddReceivedMessage(message);
					chatHubClientService.SendMessage(editTextMessage.Text, this.conversationInfoModel.InterlocutorId.ToString(), this.conversationInfoModel.ConversationId);

					editTextMessage.Text = string.Empty;
				}
				else
				{
					AlertsService.ShowToast(this, "Nie mogê po³¹czyæ siê z serwerem. Upewnij siê czy masz dostêp do internetu;");
				}
			}
		}

		private async Task GetAndSetMessages(Bundle savedInstanceState = null)
		{
			List<ConversationMessage> messages = await GetStoredOrDownloadMessages(savedInstanceState);

			if (messages != null && messages.Count > 0)
			{
				this.conversationMessagesListAdapter.AddMessages(messages);
			}
			else
			{
				//this.conversationMessagesListAdapter.Messages = new List<ConversationMessage>();
				this.conversationMessagesListAdapter.InfiniteScrollDisabled = true;
			}

			//conversationMessagesRecyclerView.SetAdapter(conversationMessagesListAdapter);
			pageNumber++;
		}

		private async Task<List<ConversationMessage>> GetStoredOrDownloadMessages(Bundle savedInstanceState)
		{
			List<ConversationMessage> messages;
			if (savedInstanceState != null)
			{
				messages = (List<ConversationMessage>)SharedObject.Data;
			}
			else
			{
				messages = await this.messagesService.GetMessages(conversationInfoModel.ConversationId, pageNumber);
			}

			return messages;
		}
	}
}