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
		Button btnSendMessage;
		ImageButton btnBack;
		EditText editTextMessage;
		int pageNumber;
		int conversationId;
		string addresseeId;
		private string bearerToken;

		public static bool IsInForeground { get; private set; }
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
			this.conversationMessagesListAdapter.AddReceivedMessage(message);
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

		protected override void OnStart()
		{
			base.OnStart();
			IsInForeground = true;
		}

		protected override void OnStop()
		{
			base.OnStop();
			IsInForeground = false;
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			SharedObject.Data = this.conversationMessagesListAdapter.Messages;
		}

		private void GetExtras()
		{
			conversationId = Intent.GetIntExtra(ExtrasKeys.CONVERSATION_ID, 0);
			addresseeId = Intent.GetStringExtra(ExtrasKeys.ADDRESSEE_ID);
		}

		private async Task SetupViews(Bundle savedInstanceState)
		{
			btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
			btnBack.Click += BtnBack_Click;
			conversationMessagesRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			btnSendMessage = FindViewById<Button>(Resource.Id.buttonSendConversationMessage);
			btnSendMessage.Click += BtnSendMessage_Click;
			editTextMessage = FindViewById<EditText>(Resource.Id.editTextConversationMessage);
			editTextMessage.KeyPress += EditTextMessage_KeyPress;
			var mLayoutManager = new LinearLayoutManager(this);
			mLayoutManager.ReverseLayout = true;
			conversationMessagesRecyclerView.SetLayoutManager(mLayoutManager);
			await GetAndSetMessages(savedInstanceState);
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
				chatHubClientService.SendMessage(editTextMessage.Text, this.addresseeId.ToString(), this.conversationId);
				var date = DateTime.Now;
				var message = new ConversationMessage();
				message.MessageContent = editTextMessage.Text;
				message.UserWasSender = true;
				message.ConversationId = this.conversationId;
				message.MessageHeader = String.Format("ja, {0} {1}", date.GetDateDottedStringFormat(), date.GetTimeColonStringFormat());
				this.conversationMessagesListAdapter.AddReceivedMessage(message);
				editTextMessage.Text = string.Empty;
			}
		}

		private async Task GetAndSetMessages(Bundle savedInstanceState = null)
		{
			List<ConversationMessage> messages = await GetStoredOrDownloadMessages(savedInstanceState);

			if (messages != null && messages.Count > 0)
			{
				if (this.conversationMessagesListAdapter == null)
				{
					this.conversationMessagesListAdapter = new ConversationMessagesListAdapter(this, messages);
				}
				else
				{
					this.conversationMessagesListAdapter.AddMessages(messages);
				}
			}
			else
			{
				if (this.conversationMessagesListAdapter == null)
				{
					this.conversationMessagesListAdapter = new ConversationMessagesListAdapter(this, new List<ConversationMessage>());
				}

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