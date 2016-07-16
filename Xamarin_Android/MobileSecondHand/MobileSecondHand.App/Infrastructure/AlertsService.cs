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
	public class AlertsService
	{
		public static void ShowToast(Context context, string message)
		{
			Toast.MakeText(context, message, ToastLength.Long).Show();
		}

		public static void ShowAlertDialog(Context context, string message)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(context);
			alert.SetTitle("Informacja");
			alert.SetMessage(message);
			alert.Show();
		}

		public static void ShowConfirmDialog(Context context, string message, Action actionOnConfirm, Action actionOnCancel = null)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(context);
			alert.SetTitle("PotwierdŸ");
			alert.SetMessage(message);
			alert.SetPositiveButton("Tak", (sender, args) =>
			{
				actionOnConfirm();
				alert.Dispose();
			});
			alert.SetNegativeButton("Nie", (sender, args) =>
			{
				if (actionOnCancel != null)
				{
					actionOnCancel();
				}

				alert.Dispose();
			});
			alert.Show();
		}

		public static void ShowSingleSelectListString(Context context, string[] items, Action<string> actionOnSelect)
		{
			var alertDialog = default(AlertDialog);
			AlertDialog.Builder buider = new AlertDialog.Builder(context);
			buider.SetTitle("Wybierz element");
			buider.SetSingleChoiceItems(items, -1, (sender, args) =>
			{
				actionOnSelect(items[args.Which]);
				alertDialog.Dismiss();
			});
			alertDialog = buider.Create();
			alertDialog.Show();
		}

		public static void ShowMultiSelectListString(Context context, string message, string[] items, List<string> userSelectedItemsNames, Action<List<string>> actionOnSelect)
		{
			var selectedItems = GetIsSelectedList(items, userSelectedItemsNames).ToArray();
			var selectAll = true;
			//do zanzaczania wszystkich

			var itemsList = items.ToList();
			itemsList.Insert(0, "Wszystko");
			items = itemsList.ToArray();

			var alertDialog = default(AlertDialog);
			AlertDialog.Builder buider = new AlertDialog.Builder(context);
			buider.SetTitle(message);

			buider.SetMultiChoiceItems(items, selectedItems, (sender, args) =>
			{
				bool isChecked = alertDialog.ListView.IsItemChecked(args.Which);
				if (args.Which == 0)
				{
					if (selectAll)
					{
						for (int i = 1; i < alertDialog.ListView.Adapter.Count; i++)
						{ // we start with first element after "Select all" choice  
							if (isChecked && !alertDialog.ListView.IsItemChecked(i)
									|| !isChecked && alertDialog.ListView.IsItemChecked(i))
							{
								alertDialog.ListView.PerformItemClick(alertDialog.ListView, i, 0);
							}
						}
					}
				}
				else
				{
					if (!isChecked && alertDialog.ListView.IsItemChecked(0))
					{
						// if other item is unselected while "Select all" is selected, unselect "Select all"   
						// false, performItemClick, true is a must in order for this code to work  
						selectAll = false;
						alertDialog.ListView.PerformItemClick(alertDialog.ListView, 0, 0);
						selectAll = true;
					}
				}
			});

			buider.SetPositiveButton("Ok", (sender, args) =>
			{
				userSelectedItemsNames.Clear();
				for (int i = 1; i < alertDialog.ListView.Adapter.Count; i++)
				{
					if (alertDialog.ListView.IsItemChecked(i))
					{
						userSelectedItemsNames.Add(items[i]);
					}
				}
				actionOnSelect(userSelectedItemsNames);
				alertDialog.Dismiss();
			});
			buider.SetNegativeButton("Anuluj", (sender, args) =>
			{
				alertDialog.Dismiss();
			});

			alertDialog = buider.Create();
			alertDialog.Show();
		}

		private static List<bool> GetIsSelectedList(string[] items, List<string> userSelectedItemsNames)
		{
			var selectedItems = new List<bool>();
			if (userSelectedItemsNames.Count == 0)
			{
				selectedItems.Add(true);
				foreach (var item in items)
				{
					selectedItems.Add(true);
				}
			}
			else
			{
				selectedItems.Add(false);
				foreach (var item in items)
				{
					selectedItems.Add(userSelectedItemsNames.Contains(item));
				}
			}

			return selectedItems;
		}
	}
}