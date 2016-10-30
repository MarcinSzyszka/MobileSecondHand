using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MobileSecondHand.API.Models.Shared.Security;
using MobileSecondHand.App.Adapters;
using MobileSecondHand.App.Consts;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.App.Infrastructure.ActivityState;
using MobileSecondHand.Services.Authentication;
using Newtonsoft.Json;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Znajd¿ u¿ytkownika")]
	public class FindUserActivity : AppCompatActivity
	{
		public const int FIND_USER_REQUEST_CODE = 10;
		private EditText textViewAutoComplete;
		private Timer timer;
		ISignInService signInService;
		string bearerToken;
		private ListView autoCompleteNamesList;
		private AutoCompleteSearchUserAdapter adapter;
		private string callingActivityName;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.FindUserActivity);
			callingActivityName = Intent.GetStringExtra(ActivityStateConsts.CALLING_ACTIVITY_NAME);
			signInService = new SignInService();
			bearerToken = (string)new SharedPreferencesHelper(this).GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			SetupViews();
			SetupTimer();
		}

		private void SetupTimer()
		{
			this.timer = new Timer(500D);
			this.timer.Elapsed += Timer_Elapsed;
		}

		private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.timer.Stop();
			var usersModelsList = await signInService.GetUserNamesModels(bearerToken, this.textViewAutoComplete.Text);
			this.RunOnUiThread(() => this.adapter.SetNewUserNamesList(usersModelsList.ToList()));
		}

		private void SetupViews()
		{
			var toolbar = FindViewById<Toolbar>(Resource.Id.app_bar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);
			toolbar.NavigationClick += Toolbar_NavigationClick;

			this.textViewAutoComplete = FindViewById<EditText>(Resource.Id.searchUserAutocompleteTextView);
			this.autoCompleteNamesList = FindViewById<ListView>(Resource.Id.listViewMatchUserNames);
			this.adapter = new AutoCompleteSearchUserAdapter(this);
			this.adapter.UserWasChosen += Adapter_UserWasChosen;
			this.autoCompleteNamesList.Adapter = adapter;
			this.textViewAutoComplete.TextChanged += TextViewAutoComplete_TextChanged;
		}

		private void Toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
		{
			Finish();
		}

		private void Adapter_UserWasChosen(object sender, UserInfoModel infoModel)
		{
			Intent myIntent;
			if (callingActivityName == ActivityStateConsts.MAIN_ACTIVITY_NAME)
			{
				myIntent = new Intent(this, typeof(MainActivity));
			}
			else
			{
				myIntent = new Intent(this, typeof(ConversationsListActivity));
			}
			var infoModelString = JsonConvert.SerializeObject(infoModel);
			myIntent.PutExtra(ActivityStateConsts.USER_INFO_MODEL, infoModelString);
			SetResult(Result.Ok, myIntent);
			Finish();
		}

		private void TextViewAutoComplete_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			timer.Stop();
			if (this.textViewAutoComplete.Text.Length > 2)
			{
				timer.Start();
			}
		}
	}
}