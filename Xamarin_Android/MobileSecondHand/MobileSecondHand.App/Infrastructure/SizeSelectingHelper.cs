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
using MobileSecondHand.API.Models.Shared.Enumerations;
using MobileSecondHand.API.Models.Shared.Extensions;

namespace MobileSecondHand.App.Infrastructure
{
	public class SizeSelectingHelper
	{
		private Context ctx;
		private ProgressDialogHelper progressDialogHelper;

		public SizeSelectingHelper(Context ctx)
		{
			this.ctx = ctx;
			this.progressDialogHelper = new ProgressDialogHelper(ctx);
		}

		public void ShowSizesListAndMakeAction(List<string> userSelectesKeywordsNames, Action<List<ClothSize>> methodToExecuteAfterCategoriesSelect)
		{
			string[] allSizesNames = GetClothSizeNames();
			Action<List<string>> action = (listString) =>
			{
				var resultList = new List<ClothSize>();
				if (listString.Count != allSizesNames.Length)
				{
					foreach (var enumValueName in listString)
					{
						resultList.Add(enumValueName.GetEnumValueByDisplayName<ClothSize>());
					}
				}
				
				methodToExecuteAfterCategoriesSelect(resultList);
			};
			AlertsService.ShowMultiSelectListString(ctx, "Wybierz rozmiary", allSizesNames, userSelectesKeywordsNames, action);
		}


		public void ShowSizeSingleSelectAndMakeAction(Action<ClothSize> actionOnSelect, string selectedItemName = null)
		{
			var allSizesNames = GetClothSizeNames();
			AlertsService.ShowSingleSelectListString(ctx, allSizesNames, (s) => actionOnSelect(s.GetEnumValueByDisplayName<ClothSize>()), selectedItemName);
		}

		private string[] GetClothSizeNames()
		{
			return Enum.GetValues(typeof(ClothSize)).GetAllItemsDisplayNames().ToArray();
		}
	}
}