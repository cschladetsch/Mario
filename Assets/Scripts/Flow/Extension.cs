//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flow
{
	public static class Extension
	{
		/// <summary>
		/// Returns true if the given sequences contains a reference to the given object.
		/// </summary>
		/// <returns>
		/// True if the sequence described by the enumerable contains a reference to the given object
		/// </returns>
		/// <param name='list'>
		/// The sequence
		/// </param>
		/// <param name='obj'>
		/// The reference to check for
		/// </param>
		/// <typeparam name='T'>
		/// The 1st type parameter.
		/// </typeparam>
		public static bool ContainsRef<T>(this IEnumerable<T> list, T obj)
		{
			return list.Any(elem => ReferenceEquals(elem, obj));
		}

		public static RectTransform GetRectTransform(this GameObject go)
		{
			return go.GetComponent<RectTransform>();
		}

		public static RectTransform GetRectTransform(this Transform go)
		{
			return go.GetComponent<RectTransform>();
		}

		/// <summary>
		/// Removes a reference from the list
		/// </summary>
		/// <param name='list'>
		/// The list to search
		/// </param>
		/// <param name='obj'>
		/// The object reference to remove.
		/// </param>
		/// <typeparam name='T'>
		/// The 1st type parameter.
		/// </typeparam>
		public static void RemoveRef<T>(this IList<T> list, T obj)
		{
			for (var n = 0; n < list.Count; ++n)
			{
				if (!ReferenceEquals(list[n], obj))
					continue;

				list.RemoveAt(n);
				return;
			}
		}
	}

	public delegate RT Func<RT>();

	public delegate RT Func<T0, RT>(T0 t0);

	public delegate RT Func<T0, T1, RT>(T0 t0, T1 t1);

	public delegate RT Func<T0, T1, T2, RT>(T0 t0, T1 t1, T2 t2);

	public delegate RT Func<T0, T1, T2, T3, RT>(T0 t0, T1 t1, T2 t2, T3 t3);

	public delegate RT Func<T0, T1, T2, T3, T4, RT>(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4);

	public delegate void Action<T0>(T0 t0);

	public delegate void Action<T0, T1>(T0 t0, T1 t1);
}