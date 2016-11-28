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
using Android.Support.V4.Content;
using MobileSecondHand.Models.Settings;
using MobileSecondHand.App.Chat;
using Android.Media;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Rozmowa", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
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
		private TextView textViewUserName;
		private CircleImageView profileImageView;
		RelativeLayout relativeLayoutSendMessage;
		private AppSettingsModel appSettings;
		private MediaPlayer player;
		private IMenu menu;

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
			this.RunOnUiThread(() =>
			{
				player.Start();
				this.conversationMessagesListAdapter.AddReceivedMessage(message);
			});
		}

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ActivityInstance = this;
			progress = new ProgressDialogHelper(this);
			signInService = new SignInService();
			bitmapService = new BitmapOperationService();
			appSettings = SharedPreferencesHelper.GetAppSettings(this);
			if (!appSettings.ChatDisabled)
			{
				this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
			}
			this.messagesService = new MessagesService(bearerToken);
			SetContentView(Resource.Layout.ConversationActivity);
			SetupViews(savedInstanceState);
			SetupConversationToolbar();
			pageNumber = 0;
			GetExtras();
			player = new MediaPlayer();
			player.SetDataSource(this, Android.Net.Uri.Parse("android.resource://" + this.PackageName + "/raw/" + Resource.Raw.message_sound));
			player.Prepare();
			progress.ShowProgressDialog("Trwa pobieranie wiadomoœci...");
			await SetupIntelocutorInfo();
			await GetAndDisplayMesages(savedInstanceState);
			coversationsLayoutWrapper.Visibility = ViewStates.Visible;
			progress.CloseProgressDialog();
		}

		protected override async void OnNewIntent(Intent intent)
		{
			try
			{
				coversationsLayoutWrapper.Visibility = ViewStates.Gone;
				pageNumber = 0;
				GetExtras(intent);
				ConversationActivity.ConversationActivityStateModel = GetStateModel(true);
				progress.ShowProgressDialog("Trwa pobieranie wiadomoœci...");
				menu.FindItem(Resource.Id.deleteConversation).SetVisible(false);
				await SetupIntelocutorInfo();
				await GetAndDisplayMesages(null);
				coversationsLayoutWrapper.Visibility = ViewStates.Visible;
				progress.CloseProgressDialog();
			}
			catch (Exception exc)
			{
				var a = exc;
			}

		}

		private async Task SetupIntelocutorInfo()
		{
			textViewUserName.Text = conversationInfoModel.InterlocutorName;
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
					else
					{
						profileImageView.SetImageDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.logo_user));
					}
				}
			}
		}

		public override void OnBackPressed()
		{
			if (!MainActivity.IsInStack)
			{
				var intent = new Intent(this, typeof(MainActivity));
				StartActivity(intent);
				Finish();
			}
			else
			{
				base.OnBackPressed();
			}
		}
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.conversationMenu, menu);
			if (menu != null)
			{
				this.menu = menu;
				menu.FindItem(Resource.Id.deleteConversation).SetVisible(false);
			}

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var handled = false;
			switch (item.ItemId)
			{
				case Resource.Id.deleteConversation:
					DeleteConversation();
					break;
			}

			return handled;
		}

		private void DeleteConversation()
		{
			Action actionOnConfirm = async () =>
			{
				this.progress.ShowProgressDialog("Trwa oznaczanie rozmowy jako usuniêtej...");
				var result = await messagesService.DeleteConversation(this.conversationInfoModel.ConversationId);
				this.progress.CloseProgressDialog();
				if (result)
				{
					AlertsService.ShowShortToast(this, "Rozmowa zosta³a oznaczona jako usuniêta.");
					this.Finish();
				}
				else
				{
					AlertsService.ShowShortToast(this, "Nie uda³o siê usun¹æ rozmowy. Spróbuj ponownie póŸniej.");
				}
			};

			AlertsService.ShowConfirmDialog(this, "Czy chcesz usun¹æ tê rozmowê? Nie bêdzie ona widoczna na liœcie rozmów dopóki Ty i Twój rozmówca nie skontaktujecie siê ponownie.", actionOnConfirm);
		}

		private void SetupConversationToolbar()
		{
			this.toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar_with_circle_image_view);
			this.profileImageView = toolbar.FindViewById<CircleImageView>(Resource.Id.profile_image_on_app_bar);
			this.textViewUserName = toolbar.FindViewById<TextView>(Resource.Id.textViewUserNameAppBar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);
			toolbar.NavigationClick += (s, e) => OnBackPressed();
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

		private void GetExtras(Intent intent = null)
		{
			var conversationInfoModelString = String.Empty;
			if (intent != null)
			{
				conversationInfoModelString = intent.GetStringExtra(ExtrasKeys.CONVERSATION_INFO_MODEL);
			}
			else
			{
				conversationInfoModelString = Intent.GetStringExtra(ExtrasKeys.CONVERSATION_INFO_MODEL);
			}

			this.conversationInfoModel = JsonConvert.DeserializeObject<ConversationInfoModel>(conversationInfoModelString);
		}

		private void SetupViews(Bundle savedInstanceState)
		{
			coversationsLayoutWrapper = FindViewById<LinearLayout>(Resource.Id.coversationsLayoutWrapper);
			coversationsLayoutWrapper.Visibility = ViewStates.Invisible;
			conversationMessagesRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			relativeLayoutSendMessage = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutBtnSendMessage);
			relativeLayoutSendMessage.Click += BtnSendMessage_Click;
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
			this.conversationMessagesListAdapter.FirstMessageAdded += ConversationMessagesListAdapter_MessageAdded;
			await GetAndSetMessages(savedInstanceState);
			this.conversationMessagesRecyclerView.SetAdapter(conversationMessagesListAdapter);
			this.conversationMessagesRecyclerView.RequestLayout();
		}

		private void ConversationMessagesListAdapter_MessageAdded(object sender, EventArgs e)
		{
			if (!menu.FindItem(Resource.Id.deleteConversation).IsVisible)
			{
				menu.FindItem(Resource.Id.deleteConversation).SetVisible(true);
			}
		}

		private void ConversationMessagesListAdapter_NewMessageAdded(object sender, EventArgs e)
		{
			this.conversationMessagesRecyclerView.SmoothScrollToPosition(0);
		}

		private void EditTextMessage_KeyPress(object sender, View.KeyEventArgs e)
		{
			if (editTextMessage.Text.IsNotNullOrEmpty() && (e.KeyCode == Keycode.DpadCenter || e.KeyCode == Keycode.Enter))
			{
				SendMessage();
				e.Handled = true;
			}
			else
			{
				e.Handled = false;
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
				if (chatHubClientService != null && chatHubClientService.IsConnected())
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
					if (appSettings.ChatDisabled)
					{
						Action actionOnConfirm = () =>
						{
							appSettings.ChatDisabled = false;
							SharedPreferencesHelper.SetAppSettings(this, appSettings);
							StartService(new Intent(this.ApplicationContext, typeof(MessengerService)));
							this.chatHubClientService = ChatHubClientService.GetServiceInstance(bearerToken);
						};

						AlertsService.ShowConfirmDialog(this, "Masz wy³¹czon¹ us³ugê czatu. Czy chcesz j¹ teraz w³¹czyæ?", actionOnConfirm);
					}
					else
					{
						AlertsService.ShowLongToast(this, "Nie mogê po³¹czyæ siê z serwerem. Upewnij siê czy masz dostêp do internetu.");

					}
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