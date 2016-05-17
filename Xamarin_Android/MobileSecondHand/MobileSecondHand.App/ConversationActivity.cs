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
using MobileSecondHand.Models.Chat;

namespace MobileSecondHand.App {
	[Activity(Label = "ConversationActivity")]
	public class ConversationActivity : Activity {
		ConversationMessagesListAdapter conversationMessagesListAdapter;
		ConversationMessage message;
		RecyclerView conversationMessagesRecyclerView;
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.ConversationActivity);
			var messageConetnt = Intent.GetStringExtra("a");
			message = new ConversationMessage { MessageContent = messageConetnt, MessageHeader = "Serwer, 17.05.2016", UserWasSender = false };
			SetupViews();
		}

		private void SetupViews() {
			conversationMessagesRecyclerView = FindViewById<RecyclerView>(Resource.Id.conversationsRecyclerView);
			var mLayoutManager = new LinearLayoutManager(this);
			conversationMessagesRecyclerView.SetLayoutManager(mLayoutManager);
			var newMes = new ConversationMessage { MessageContent = "Jakaœ trêsc wiadomoœci bardzo interesuj¹ca", MessageHeader = "Ja, 17.05.2016", UserWasSender = true };
			this.conversationMessagesListAdapter = new ConversationMessagesListAdapter(new List<ConversationMessage> { message, newMes, message, newMes, message, newMes, message, newMes});
			conversationMessagesRecyclerView.SetAdapter(conversationMessagesListAdapter);
		}
	}
}