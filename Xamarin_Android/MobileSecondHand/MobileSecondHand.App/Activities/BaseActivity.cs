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
		private DrawerLayout drawerLayout;
		NavigationViewMenu navigationViewMenu;
		protected SharedPreferencesHelper sharedPreferencesHelper;
		protected string bearerToken;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(this);
			bearerToken = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
		}

		protected void SetupToolbar(bool navigationVisible = true)
		{
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

			var toolbar = FindViewById<Toolbar>(Resource.Id.app_bar);
			SetSupportActionBar(toolbar);


			//SupportActionBar.SetTitle(Resource.String.app);

			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);

			// Create ActionBarDrawerToggle button and add it to the toolbar
			var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
			drawerLayout.SetDrawerListener(drawerToggle);
			drawerToggle.SyncState();
			drawerLayout.DrawerOpened += DrawerLayout_DrawerOpened;
			if (drawerLayout.IsDrawerOpen(FindViewById(Resource.Id.nav_view)))
			{
				SetupSideMenu();
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			var settingsModel = (AppSettingsModel)this.sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
			if (settingsModel != null)
			{
				if (!settingsModel.ChatDisabled && !MessengerService.ServiceIsRunning)
				{
					StartService(new Intent(this.BaseContext, typeof(MessengerService)));
					ActivityInstancesWhichStartedServices.ActivityWhichStartedMessengerService = this.BaseContext;
				}
				if (!settingsModel.NotificationsDisabled && !NewsService.ServiceIsRunning)
				{
					StartService(new Intent(this.BaseContext, typeof(NewsService)));
					ActivityInstancesWhichStartedServices.ActivityWhichStartedNotificationsService = this.BaseContext;
				}
			}

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

		private void SetupSideMenu()
		{
			if (navigationViewMenu == null)
			{
				navigationViewMenu = new NavigationViewMenu(this, this.sharedPreferencesHelper);
			}

			navigationViewMenu.SetupMenu();
		}

		private void DrawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
		{
			SetupSideMenu();
		}

		public override void OnBackPressed()
		{
			if (drawerLayout.IsDrawerOpen(FindViewById(Resource.Id.nav_view)))
			{
				drawerLayout.CloseDrawers();
			}
			else
			{
				base.OnBackPressed();
			}

		}
	}
}