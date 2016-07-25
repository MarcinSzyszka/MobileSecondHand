using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.Common.Enumerations;

namespace MobileSecondHand.Common.Extensions
{
	public static class EnumExtensions
	{
		public static string GetDisplayName(this Enum value)
		{
			Type type = value.GetType();
			var attribute = type.GetTypeInfo().DeclaredMembers.FirstOrDefault(m => m.Name == value.ToString()).GetCustomAttribute<DisplayNameAttribute>();

			return attribute != null ? attribute.DisplayName : "";
		}

		public static List<string> GetAllItemsDisplayNames(this Array value)
		{
			var itemsNames = new List<string>();

			foreach (Enum item in value)
			{
				var attribute = item.GetType().GetTypeInfo().DeclaredMembers.FirstOrDefault(m => m.Name == item.ToString()).GetCustomAttribute<DisplayNameAttribute>();
				if (attribute != null)
				{
					itemsNames.Add(attribute.DisplayName);
				}
				else
				{
					continue;
				}
			}

			return itemsNames;
		}
		public static T GetEnumValueByDisplayName<T>(this string displayName)
		{
			Type enumType = typeof(T);

			foreach (var member in enumType.GetTypeInfo().DeclaredMembers)
			{
				var attribute = member.GetCustomAttribute<DisplayNameAttribute>();
				if (attribute != null && attribute.DisplayName == displayName)
				{

					return (T)Enum.Parse(typeof(T), member.Name);
				}
			}

			return default(T);
		}

	}
}
