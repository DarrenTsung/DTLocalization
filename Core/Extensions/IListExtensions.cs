using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTLocalization.Internal {
	public static class IListExtensions {
		public static bool ContainsIndex(this IList list, int index) {
			return index >= 0 && index < list.Count;
		}

		public static bool IsNullOrEmpty(this IList list) {
			return list == null || list.Count == 0;
		}

		public static int ClampIndex(this IList l, int i) {
			return MathUtil.Clamp(i, 0, l.Count - 1);
		}

		public static int WrapIndex(this IList l, int i) {
			return MathUtil.Wrap(i, 0, l.Count);
		}

		// NOTE (darren): functions that are the same between IList<T> / IList
		// will conflict because arrays / lists are both - hide one version since we
		// only need two versions in this class
		private static bool ContainsIndex<T>(this IList<T> list, int index) {
			return index >= 0 && index < list.Count;
		}

		private static bool IsNullOrEmpty<T>(this IList<T> list) {
			return list == null || list.Count == 0;
		}

		private static int WrapIndex<T>(this IList<T> l, int i) {
			return MathUtil.Wrap(i, 0, l.Count);
		}

		private static int ClampIndex<T>(this IList<T> l, int i) {
			return MathUtil.Clamp(i, 0, l.Count - 1);
		}
	}
}
