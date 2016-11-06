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
using Refractored.Controls;
using MobileSecondHand.API.Models.Shared.Chat;
using MobileSecondHand.Services.Authentication;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Rozmowa")]
	public class ConversationActivity : BaseActivity, IInfiniteScrollListener
	{
		ISignInService signInService;
		IMessagesService messagesService;
		BitmapOperationService bitmapService;
		ConversationMessagesListAdapter conversationMessagesListAdapter;
		ChatHubClientService chatHubClientService;
		RecyclerView conversationMessagesRecyclerView;
		ImageButton btnSendMessage;
		EditText editTextMessage;
		LinearLayout coversationsLayoutWrapper;
		ConversationInfoModel conversationInfoModel;
		int pageNumber;
		private ProgressDialogHelper progress;

		public static ConversationActivityStateModel ConversationActivityStateModel { get; private set; } = new ConversationActivityStateModel(false, 0);
		public static ConversationActivity ActivityInstance { get; private set; }

		public async void OnInfiniteScroll()
		{
			progress.ShowProgressDialog("Trwa pobieranie wiadomoœci...");
			await GetAndSetMessages();
			progress.CloseProgressDialog();
		}

		public void AddMessage(ConversationMessage message)
		{
			this.RunOnUiThread(() => this.conversationMessagesListAdapter.AddReceivedMessage(message));
		}

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ActivityInstance = this;
			progress = new ProgressDialogHelper(this);
			signInService = new SignInService();
			bitmapService = new BitmapOperationService();
			this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
			this.messagesService = new MessagesService(bearerToken);
			SetContentView(Resource.Layout.ConversationActivity);
			SetupViews(savedInstanceState);
			GetExtras();
			progress.ShowProgressDialog("Trwa pobieranie wiadomoœci...");
			await SetupToolbarWithProfileImage();
			pageNumber = 0;
			await GetAndDisplayMesages(savedInstanceState);
			coversationsLayoutWrapper.Visibility = ViewStates.Visible;
			progress.CloseProgressDialog();
		}

		private async Task SetupToolbarWithProfileImage()
		{
			this.toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar_with_circle_image_view);
			var profileImageView = toolbar.FindViewById<CircleImageView>(Resource.Id.profile_image_on_app_bar);
			if (conversationInfoModel.InterlocutorPrifileImage != null)
			{
				if (conversationInfoModel.InterlocutorPrifileImage.Length > 0)
				{
					profileImageView.SetImageBitmap(await this.bitmapService.GetScaledDownBitmapForDisplayAsync(conversationInfoModel.InterlocutorPrifileImage));
				}
				else
				{
					var imageBytes = await signInService.GetUserProfileImage(bearerToken, conversationInfoModel.InterlocutorId);
					if (imageBytes != null)
					{
						profileImageView.SetImageBitmap(await this.bitmapService.GetScaledDownBitmapForDisplayAsync(imageBytes));
					}
				}

			}
			var textViewUserName = toolbar.FindViewById<TextView>(Resource.Id.textViewUserNameAppBar);
			textViewUserName.Text = conversationInfoModel.InterlocutorName;
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);
			toolbar.NavigationClick += (s, e) => this.Finish();
		}

		//public override bool OnCreateOptionsMenu(IMenu menu)
		//{
		//	MenuInflater.Inflate(Resource.Menu.mainActivityMenu, menu);
		//	if (menu != null)
		//	{
		//		menu.FindItem(Resource.Id.applyFilterOptions).SetVisible(false);
		//		menu.FindItem(Resource.Id.clearFilterOptions).SetVisible(false);
		//		menu.FindItem(Resource.Id.refreshAdvertisementsOption).SetVisible(false);
		//		menu.FindItem(Resource.Id.chat).SetVisible(true);
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

		private void SetupViews(Bundle savedInstanceState)
		{
			coversationsLayoutWrapper = FindViewById<LinearLayout>(Resource.Id.coversationsLayoutWrapper);
			coversationsLayoutWrapper.Visibility = ViewStates.Invisible;
			conversationMessagesRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			btnSendMessage = FindViewById<ImageButton>(Resource.Id.buttonSendConversationMessage);
			btnSendMessage.Click += BtnSendMessage_Click;
			editTextMessage = FindViewById<EditText>(Resource.Id.editTextConversationMessage);
			editTextMessage.KeyPress += EditTextMessage_KeyPress;
			var mLayoutManager = new LinearLayoutManager(this);
			mLayoutManager.ReverseLayout = true;
			mLayoutManager.SmoothScrollbarEnabled = true;
			conversationMessagesRecyclerView.SetLayoutManager(mLayoutManager);
		}

		private async Task GetAndDisplayMesages(Bundle savedInstanceState)
		{
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
					AlertsService.ShowLongToast(this, "Nie mogê po³¹czyæ siê z serwerem. Upewnij siê czy masz dostêp do internetu;");
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