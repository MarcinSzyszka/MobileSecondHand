using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileSecondHand.App.Consts;
using MobileSecondHand.Services.Categories;

namespace MobileSecondHand.App.Infrastructure
{
	public class CategoriesSelectingHelper
	{
		private Context ctx;
		private ProgressDialogHelper progressDialogHelper;
		ICategoryService categoryService;

		public CategoriesSelectingHelper(Context ctx, string bearerToken)
		{
			this.ctx = ctx;
			this.progressDialogHelper = new ProgressDialogHelper(ctx);
			var token = bearerToken;
			this.categoryService = new CategoryService(token);
		}

		public async Task ShowCategoriesListAndMakeAction(List<string> userSelectesKeywordsNames, Func<IDictionary<int, string>, Action<List<string>>> methodToExecuteAfterCategoriesSelect)
		{
			try
			{
				this.progressDialogHelper.ShowProgressDialog("Trwa pobieranie danych");
				var allKeywords = await this.categoryService.GetCategories();
				var allKeywordsNames = allKeywords.Select(k => k.Value).ToArray();
				this.progressDialogHelper.CloseProgressDialog();
				AlertsService.ShowMultiSelectListString(ctx, "Wybierz kategorie", allKeywordsNames, userSelectesKeywordsNames, methodToExecuteAfterCategoriesSelect(allKeywords));
			}
			catch (Exception exc)
			{
				this.progressDialogHelper.CloseProgressDialog();
				AlertsService.ShowLongToast(ctx, "Wyst�pi� problem z pobraniem danych. Upewnij si�, �e masz dost�p do internetu");
			}
		}


		public async Task ShowCategoriesSingleSelectAndMakeAction(Action<int, string> actionOnSelect, string selectedItemName = null)
		{
			try
			{
				this.progressDialogHelper.ShowProgressDialog("Trwa pobieranie danych");
				var allKeywords = await this.categoryService.GetCategories();
				var allKeywordsNames = allKeywords.Select(k => k.Value).ToArray();
				this.progressDialogHelper.CloseProgressDialog();
				AlertsService.ShowSingleSelectListString(ctx, allKeywordsNames, s => actionOnSelect(allKeywords.First(v => v.Value == s).Key, s), selectedItemName, "Wybierz kategori�");

			}
			catch (Exception exc)
			{
				AlertsService.ShowLongToast(ctx, "Wyst�pi� problem z pobraniem danych. Upewnij si�, �e masz dost�p do internetu: " + exc);
				this.progressDialogHelper.CloseProgressDialog();
			}
		}
	}
}