using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTLocalization.Internal {
	public static class IEnumerableGenericExtensions {
		public static IEnumerable<T> Yield<T>(this T item) {
			yield return item;
		}
	}
}