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

namespace MobileSecondHand.App.Infrastructure.ActivityState
{
	public class ActivityStateConsts
	{
		public const string SELECTED_PHOTO_INDEX_TO_START_ON_PHOTOS_VIEWER = "selectedPhotoIndexToStartOnPhotosViewer";
		public const string CALLING_ACTIVITY_NAME = "CallingActivityName";
		public const string MAIN_ACTIVITY_NAME = "MainActivity";
		public const string CONVERSATIONS_ACTIVITY_NAME = "ConversationsActivity";
		public const string USER_INFO_MODEL = "UserInfoModel";
	}
}