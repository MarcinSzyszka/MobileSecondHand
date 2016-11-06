using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Chat;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.App.Notifications;
using MobileSecondHand.App.SideMenu;
using MobileSecondHand.Models.Settings;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "BaseActivityWithNavigationDrawer")]
	public class BaseActivityWithNavigationDrawer : BaseActivity
	{
		private DrawerLayout drawerLayout;
		protected NavigationViewMenu navigationViewMenu;
		protected ActionBarDrawerToggle drawerToggle;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		protected override void OnStart()
		{
			base.OnStart();
			var settingsModel = (AppSettingsModel)this.sharedPreferencesHelper.GetSharedPreference<AppSettingsModel>(SharedPreferencesKeys.APP_SETTINGS);
			if (settingsModel != null)
			{
				if (!settingsModel.ChatDisabled && !MessengerService.ServiceIsRunning)
				{
					StartService(new Intent(this.ApplicationContext, typeof(MessengerService)));
				}
				if (!settingsModel.NotificationsDisabled && !NewsService.ServiceIsRunning)
				{
					StartService(new Intent(this.ApplicationContext, typeof(NewsService)));
				}
			}

		}


		protected void SetupDrawer()
		{
			if (drawerToggle == null)
			{
				drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
				drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
				drawerLayout.SetDrawerListener(drawerToggle);
				drawerToggle.SyncState();
				drawerLayout.DrawerOpened += DrawerLayout_DrawerOpened;
			}
			else
			{
				drawerToggle.SyncState();
			}

		}

		private void SetupSideMenu()
		{
			ScrollView navView = null;
			var isFirstTimeOpened = false;
			if (navigationViewMenu == null)
			{
				isFirstTimeOpened = true;
				navView = FindViewById<ScrollView>(Resource.Id.navWievSvrollLayout);
				navigationViewMenu = new NavigationViewMenu(this, this.sharedPreferencesHelper);
			}

			navigationViewMenu.SetupMenu();
			if (isFirstTimeOpened)
			{
				navView.Visibility = ViewStates.Visible;
			}
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