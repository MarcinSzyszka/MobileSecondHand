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
using MobileSecondHand.App.Receivers;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "BaseActivityWithNavigationDrawer")]
	public class BaseActivityWithNavigationDrawer : BaseActivity
	{
		private DrawerLayout drawerLayout;
		protected NavigationViewMenu navigationViewMenu;
		protected ActionBarDrawerToggle drawerToggle;
		public static bool IsInStack { get; private set; }
		public override void OnBackPressed()
		{
			if (IsDrawerOpen())
			{
				drawerLayout.CloseDrawers();
			}
			else
			{
				base.OnBackPressed();
			}

		}
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}
		protected override void OnDestroy()
		{
			IsInStack = false;
			base.OnDestroy();
		}

		protected override void OnStart()
		{
			base.OnStart();
			var settingsModel = SharedPreferencesHelper.GetAppSettings(this);
			if (settingsModel != null)
			{
				WakeUpAlarmReceiver.SetWakeUpAlarmRepeating(this.ApplicationContext, AlarmManager.IntervalHour);
				if (!settingsModel.ChatDisabled && !MessengerService.ServiceIsRunning)
				{
					StartService(new Intent(this.ApplicationContext, typeof(MessengerService)));
				}
			}
			IsInStack = true;
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

		protected bool IsDrawerOpen()
		{
			return drawerLayout.IsDrawerOpen(FindViewById(Resource.Id.nav_view));
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

		

	}
}