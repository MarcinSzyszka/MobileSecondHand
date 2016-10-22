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
using MobileSecondHand.API.Models.Shared.Security;

namespace MobileSecondHand.App.Adapters
{
	public class AutoCompleteSearchUserAdapter : BaseAdapter<UserInfoModel>
	{
		private Activity activity;
		private List<UserInfoModel> userInfoModels;
		public event EventHandler<UserInfoModel> UserWasChosen;

		public AutoCompleteSearchUserAdapter(Activity activity)
		{
			this.userInfoModels = new List<UserInfoModel>(); ;
			this.activity = activity;
		}
		public void SetNewUserNamesList(List<UserInfoModel> userInfoModels)
		{
			this.userInfoModels = userInfoModels;
			this.NotifyDataSetChanged();
		}
		public override UserInfoModel this[int position]
		{
			get { return userInfoModels[position]; }
		}

		public override int Count
		{
			get { return userInfoModels.Count; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.MatchUserNameRow, parent, false);
			var nameTextView = view.FindViewById<TextView>(Resource.Id.testViewMatchUserName);
			nameTextView.Text = userInfoModels[position].UserName;
			view.Click += View_Click;
			return view;
		}

		private void View_Click(object sender, EventArgs e)
		{
			if (UserWasChosen != null)
			{
				var view = sender as View;
				var nameTextView = view.FindViewById<TextView>(Resource.Id.testViewMatchUserName);
				UserWasChosen(this, userInfoModels.First(u => u.UserName == nameTextView.Text));
			}
		}
	}
}