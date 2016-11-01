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

namespace MobileSecondHand.App.Infrastructure
{
	public class ProgressDialogHelper
	{
		private ProgressDialog progress;
		public event EventHandler progresShow;
		public ProgressDialogHelper(Context context)
		{
			this.progress = new ProgressDialog(context);
			progress.ShowEvent += Progress_ShowEvent;
			progress.SetProgressStyle(ProgressDialogStyle.Spinner);
			progress.Indeterminate = false;
			progress.Progress = 99;
			progress.SetCanceledOnTouchOutside(false);
		}

		private void Progress_ShowEvent(object sender, EventArgs e)
		{
			if (progresShow != null)
			{
				progresShow(sender, e);
			}
		}

		public void ShowProgressDialog(string message)
		{
			progress.SetMessage(message);
			progress.Show();
		}

		public void CloseProgressDialog()
		{
			if (progress.IsShowing)
			{
				progress.Hide();
			}

		}
	}
}