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
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.SideMenu;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "BaseActivity")]
	public class BaseActivity : AppCompatActivity
	{
		private DrawerLayout drawerLayout;
		NavigationViewMenu navigationViewMenu;
		SharedPreferencesHelper sharedPreferencesHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(this);
		}

		protected void SetupToolbar()
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