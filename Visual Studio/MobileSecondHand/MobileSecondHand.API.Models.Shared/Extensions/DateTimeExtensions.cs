using System;

namespace MobileSecondHand.API.Models.Shared.Extensions
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Method return date in format dd.mm.yyy
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string GetDateDottedStringFormat(this DateTime date)
		{
			var day = date.Day < 10 ? String.Format("0{0}", date.Day) : date.Day.ToString();
			var month = date.Month < 10 ? String.Format("0{0}", date.Month) : date.Month.ToString();

			return String.Format("{0}.{1}.{2}", day, month, date.Year);
		}

		/// <summary>
		/// Method returns time in format hh:mm:ss
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string GetTimeColonStringFormat(this DateTime date)
		{
			var hour = date.Hour < 10 ? String.Format("0{0}", date.Hour) : date.Hour.ToString();
			var min = date.Minute < 10 ? String.Format("0{0}", date.Minute) : date.Minute.ToString();
			var sec = date.Second < 10 ? String.Format("0{0}", date.Second) : date.Second.ToString();

			return String.Format("{0}:{1}:{2}", hour, min, sec);
		}
	}
}
