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
		private SharedPreferencesHelper sharedPreferencesHelper;
		ICategoryService categoryService;

		public CategoriesSelectingHelper(Context ctx)
		{
			this.ctx = ctx;
			this.progressDialogHelper = new ProgressDialogHelper(ctx);
			this.sharedPreferencesHelper = new SharedPreferencesHelper(ctx);
			var token = (string)this.sharedPreferencesHelper.GetSharedPreference<string>(SharedPreferencesKeys.BEARER_TOKEN);
			this.categoryService = new CategoryService(token);
		}

		internal async Task ShowCategoriesListAndMakeAction(List<string> userSelectesKeywordsNames, Func<IDictionary<int, string>, Action<List<string>>> methodToExecuteAfterCategoriesSelect)
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
				AlertsService.ShowToast(ctx, "Wyst¹pi³ problem z pobraniem danych. Upewnij siê, ¿e masz dostêp do internetu");
			}
			finally
			{
				this.progressDialogHelper.CloseProgressDialog();
			}
		}


		internal async Task ShowCategoriesSingleSelectAndMakeAction(Action<int, string> actionOnSelect, string selectedItemName = null)
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
				AlertsService.ShowToast(ctx, "Wyst¹pi³ problem z pobraniem danych. Upewnij siê, ¿e masz dostêp do internetu");
			}
			finally
			{
				this.progressDialogHelper.CloseProgressDialog();
			}
		}
	}
}