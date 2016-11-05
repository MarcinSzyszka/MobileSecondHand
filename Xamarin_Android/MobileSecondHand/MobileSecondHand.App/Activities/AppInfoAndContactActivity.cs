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
using MobileSecondHand.API.Models.Shared.Feedback;
using MobileSecondHand.App.Infrastructure;
using MobileSecondHand.Services.Feedback;

namespace MobileSecondHand.App.Activities
{
	[Activity(Label = "Informacje o aplikacji", ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
	public class AppInfoAndContactActivity : BaseActivity
	{
		ProgressDialogHelper progress;
		private FeedbackService feedbackService;
		RelativeLayout infoLayout;
		RelativeLayout contactLayout;
		Button btnSendFeedback;
		Spinner messageType;
		EditText telModel;
		EditText messageINfoContet;
		Button btnSubmitSenInfo;
		TextView textViewAppVersion;
		private string messageTypeStringContent;
		private int selectedMessageTypeItemPosition;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.AppInfoAndContactActivity);
			feedbackService = new FeedbackService(bearerToken);
			progress = new ProgressDialogHelper(this);
			base.SetupToolbar();
			SetupViews();
		}

		private void SetupViews()
		{
			this.textViewAppVersion = FindViewById<TextView>(Resource.Id.textViewAppVersion);
			var version = this.ApplicationContext.PackageManager.GetPackageInfo(this.ApplicationContext.PackageName, 0).VersionName;
			this.textViewAppVersion.Text = String.Format("Wersja aplikacji: {0}", version);
			this.infoLayout = FindViewById<RelativeLayout>(Resource.Id.appInfoLayout);
			this.contactLayout = FindViewById<RelativeLayout>(Resource.Id.contactLayout);
			this.btnSendFeedback = FindViewById<Button>(Resource.Id.btnSendFeedback);
			this.btnSendFeedback.Click += (s, e) =>
			{
				TogleLayouts();
			};
			this.btnSubmitSenInfo = FindViewById<Button>(Resource.Id.btnSubmitSenInfo);
			btnSubmitSenInfo.Click += async (s, e) =>
			{
				if (selectedMessageTypeItemPosition == 0 && String.IsNullOrEmpty(telModel.Text))
				{
					AlertsService.ShowShortToast(this, "Podaj model telefonu");
					return;
				}
				if (String.IsNullOrEmpty(messageINfoContet.Text))
				{
					AlertsService.ShowShortToast(this, "WprowadŸ treœæ wiadomoœci.");
					return;
				}
				progress.ShowProgressDialog("Wysy³anie zg³oszenia...");
				var model = new NotificationFromUser();
				model.Title = messageTypeStringContent;
				model.TelModel = this.telModel.Text;
				model.MessageContent = this.messageINfoContet.Text;
				progress.CloseProgressDialog();
				var success = await this.feedbackService.SendNotificationFromUser(model);
				if (success)
				{
					AlertsService.ShowLongToast(this, "Zg³oszenie zosta³o wys³ane. Dziêkujemy.");
					ClearViews();
					OnBackPressed();
				}
				else
				{
					AlertsService.ShowShortToast(this, "Nie uda³o siê wys³aæ zg³oszenia.");
				}
			};
			this.messageType = FindViewById<Spinner>(Resource.Id.messageType);
			this.telModel = FindViewById<EditText>(Resource.Id.telModel);
			this.messageINfoContet = FindViewById<EditText>(Resource.Id.messageINfoContet);

			SetupSpinner();
		}

		private void ClearViews()
		{
			this.telModel.Text = "";
			this.messageINfoContet.Text = "";
			this.messageTypeStringContent = messageType.GetItemAtPosition(messageType.SelectedItemPosition).ToString();
		}

		private void TogleLayouts()
		{
			if (this.infoLayout.Visibility == ViewStates.Visible)
			{
				toolbar.Title = "Kontakt";
				this.infoLayout.Visibility = ViewStates.Gone;
				this.contactLayout.Visibility = ViewStates.Visible;
			}
			else
			{
				toolbar.Title = "Informacje o aplikacji";
				this.infoLayout.Visibility = ViewStates.Visible;
				this.contactLayout.Visibility = ViewStates.Gone;
			}
		}

		public override void OnBackPressed()
		{
			if (this.contactLayout.Visibility == ViewStates.Visible)
			{
				TogleLayouts();
			}
			else
			{
				base.OnBackPressed();
			}

		}

		protected override void Toolbar_NavigationClick(object sender, Android.Support.V7.Widget.Toolbar.NavigationClickEventArgs e)
		{
			OnBackPressed();
		}

		private void SetupSpinner()
		{
			messageType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.message_info_from_user, Resource.Layout.messageTypeSpinnerSimpleItem);
			adapter.SetDropDownViewResource(Resource.Layout.messageTypeSpinnerDropdownItem);
			messageType.Adapter = adapter;
		}

		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			messageTypeStringContent = spinner.GetItemAtPosition(e.Position).ToString();
			selectedMessageTypeItemPosition = e.Position;
			if (e.Position > 0)
			{
				this.telModel.Visibility = ViewStates.Gone;
			}
			else
			{
				this.telModel.Visibility = ViewStates.Visible;
			}
		}
	}
}