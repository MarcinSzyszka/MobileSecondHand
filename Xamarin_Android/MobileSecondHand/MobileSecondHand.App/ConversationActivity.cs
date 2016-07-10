using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Models.Chat;
using MobileSecondHand.Services.Chat;
using MobileSecondHand.Common.Extensions;
using MobileSecondHand.App.Infrastructure.ActivityState;
using Android.Content.Res;

namespace MobileSecondHand.App
{
	[Activity(Label = "ConversationActivity")]
	public class ConversationActivity : Activity, IInfiniteScrollListener
	{
		IMessagesService messagesService;
		SharedPreferencesHelper preferenceHelper;
		ConversationMessagesListAdapter conversationMessagesListAdapter;
		ChatHubClientService chatHubClientService;
		RecyclerView conversationMessagesRecyclerView;
		ImageButton btnSendMessage;
		EditText editTextMessage;
		int pageNumber;
		int conversationId;
		string addresseeId;
		private string bearerToken;

		public static ConversationActivityStateModel ConversationActivityStateModel { get; private set; } = new ConversationActivityStateModel(false, 0);
		public static ConversationActivity ActivityInstance { get; private set; }

		public ConversationActivity()
		{
			this.messagesService = new MessagesService();
		}

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
			this.preferenceHelper = new SharedPreferencesHelper(this);
			bearerToken = (string)preferenceHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
			SetContentView(Resource.Layout.ConversationActivity);
			GetExtras();
			pageNumber = 0;
			await SetupViews(savedInstanceState);
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
			return new ConversationActivityStateModel(isInForeground, this.conversationId);
		}

		private void GetExtras()
		{
			conversationId = Intent.GetIntExtra(ExtrasKeys.CONVERSATION_ID, 0);
			addresseeId = Intent.GetStringExtra(ExtrasKeys.ADDRESSEE_ID);
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
			this.conversationMessagesRecyclerView.RequestLayout();
		}

		private void ConversationMessagesListAdapter_NewMessageAdded(object sender, EventArgs e)
		{
			this.conversationMessagesRecyclerView.SmoothScrollToPosition(0);
		}

		private void BtnBack_Click(object sender, EventArgs e)
		{
			this.Finish();
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
					message.ConversationId = this.conversationId;
					message.MessageHeader = String.Format("ja, {0} {1}", date.GetDateDottedStringFormat(), date.GetTimeColonStringFormat());

					this.conversationMessagesListAdapter.AddReceivedMessage(message);
					chatHubClientService.SendMessage(editTextMessage.Text, this.addresseeId.ToString(), this.conversationId);

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

			conversationMessagesRecyclerView.SetAdapter(conversationMessagesListAdapter);
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
				messages = await this.messagesService.GetMessages(conversationId, pageNumber, this.bearerToken);
			}

			return messages;
		}
	}
}