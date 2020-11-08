using System;
using System.Collections.Generic;
using System.Linq;

namespace SweatyChair.TreeView
{
	public static class TableViewExtension
	{

		#region Ordering / Sorting

		public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
				return source.OrderBy(selector);
			else
				return source.OrderByDescending(selector);
		}

		public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
				return source.ThenBy(selector);
			else
				return source.ThenByDescending(selector);
		}

		#endregion

	}

}