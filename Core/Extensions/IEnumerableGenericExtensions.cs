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

		public static Dictionary<TKey, TValue> ToMap<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keyTransformation, Func<T, TValue> valueTransformation) {
			Dictionary<TKey, TValue> map = new Dictionary<TKey, TValue>();
			foreach (T element in enumerable) {
				TKey key = keyTransformation.Invoke(element);
				if (map.ContainsKey(key)) {
					Debug.LogWarning("ToMapWithKeys - key (" + key + ") appears twice, will override the previous value: " + map[key]);
				}

				TValue value = valueTransformation.Invoke(element);
				map[key] = value;
			}
			return map;
		}
	}
}