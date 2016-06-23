using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

namespace MobileSecondHand.App {
	[Activity(Label = "ConversationActivity")]
	public class ConversationActivity : Activity {
		IMessagesService messagesService;
		SharedPreferencesHelper preferenceHelper;
		ConversationMessagesListAdapter conversationMessagesListAdapter;
		ConversationMessage message;
		RecyclerView conversationMessagesRecyclerView;
		int pageNumber;

		public ConversationActivity() {
			this.messagesService = new MessagesService();
		}

		protected override async void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			preferenceHelper = new SharedPreferencesHelper(this);
			SetContentView(Resource.Layout.ConversationActivity);

			var conversationId = Intent.GetStringExtra(ExtrasKeys.CONVERSATION_ID);
			pageNumber = 0;
			
			var bearerToken = (string)preferenceHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			List<ConversationMessage> messages = await this.messagesService.GetMessages(conversationId, pageNumber, bearerToken);

			message = new ConversationMessage { MessageContent = messageConetnt, MessageHeader = "Serwer, 17.05.2016", UserWasSender = false };
			SetupViews();
		}

		private void SetupViews() {
			conversationMessagesRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			var mLayoutManager = new LinearLayoutManager(this);
			conversationMessagesRecyclerView.SetLayoutManager(mLayoutManager);
			var newMes = new ConversationMessage { MessageContent = "Jakaœ treœæ wiadomoœci bardzo interesuj¹ca", MessageHeader = "Ja, 17.05.2016", UserWasSender = true };
			this.conversationMessagesListAdapter = new ConversationMessagesListAdapter(new List<ConversationMessage> { message, newMes, message, newMes, message, newMes, message, newMes});
			conversationMessagesRecyclerView.SetAdapter(conversationMessagesListAdapter);
		}
	}
}