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
			this.progressDialogHelper.ShowProgressDialog("Trwa pobieranie danych");
			try
			{
				var allKeywords = await this.categoryService.GetCategories();
				var allKeywordsNames = allKeywords.Select(k => k.Value).ToArray();

				AlertsService.ShowMultiSelectListString(ctx, "Wybierz kategorie", allKeywordsNames, userSelectesKeywordsNames, methodToExecuteAfterCategoriesSelect(allKeywords));

			}
			catch (Exception exc)
			{
				AlertsService.ShowLongToast(ctx, "Wyst¹pi³ problem z pobraniem danych. Upewnij siê, ¿e masz dostêp do internetu");
			}
			finally
			{
				this.progressDialogHelper.CloseProgressDialog();
			}
		}


		public async Task ShowCategoriesSingleSelectAndMakeAction(Action<int, string> actionOnSelect, string selectedItemName = null)
		{
			this.progressDialogHelper.ShowProgressDialog("Trwa pobieranie danych");
			try
			{
				var allKeywords = await this.categoryService.GetCategories();
				var allKeywordsNames = allKeywords.Select(k => k.Value).ToArray();

				AlertsService.ShowSingleSelectListString(ctx, allKeywordsNames, s => actionOnSelect(allKeywords.First(v => v.Value == s).Key, s) , selectedItemName);

			}
			catch (Exception exc)
			{
				AlertsService.ShowLongToast(ctx, "Wyst¹pi³ problem z pobraniem danych. Upewnij siê, ¿e masz dostêp do internetu");
			}
			finally
			{
				this.progressDialogHelper.CloseProgressDialog();
			}
		}
	}
}