using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.App.Notifications;
using MobileSecondHand.App.SideMenu;
using MobileSecondHand.Models.Settings;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MobileSecondHand.App.Activities
{
	[Activity]
	public class BaseActivity : AppCompatActivity
	{
		protected SharedPreferencesHelper sharedPreferencesHelper;
		protected string bearerToken;
		protected Toolbar toolbar;
		public string BearerToken { get { return bearerToken; } }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(this);
			bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
		}

		protected void SetupToolbar()
		{
			this.toolbar = FindViewById<Toolbar>(Resource.Id.app_bar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);
			toolbar.NavigationClick += Toolbar_NavigationClick;
		}


		protected virtual void Toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
		{
			Finish();
		}

		protected void GoToMainPage()
		{
			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
		}

		protected void GoToChat()
		{
			var intent = new Intent(this, typeof(ConversationsListActivity));
			StartActivity(intent);
		}


	}
}