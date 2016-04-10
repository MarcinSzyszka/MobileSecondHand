using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using MobileSecondHand.App.Infrastructure;

namespace MobileSecondHand.App {
	[Activity(Label = "MainActivity")]
	public class MainActivity : Activity {
		private ListView advertisementsListView;

		public MainActivity() {

		}

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.MainActivity);
			SetupViews();
			// Create your application here
		}

		private void SetupViews() {
			SetupFab();
			advertisementsListView = FindViewById<ListView>(Resource.Id.advertisementsListView);
		}

		private void SetupFab() {
			var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += Fab_Click;
		}

		private void Fab_Click(object sender, EventArgs e) {
			AlertsService.ShowToast(this, "Bum!");
		}
	}
}