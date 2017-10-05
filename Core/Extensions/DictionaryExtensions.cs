using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTLocalization.Internal {
	public static class DictionaryExtensions {
		public static V SafeGet<U, V>(this IDictionary<U, V> source, U key, V defaultValue = default(V)) {
			if (key == null) {
				Debug.LogWarning("Key is null - not good!");
				return defaultValue;
			}

			if (source.ContainsKey(key)) {
				return source[key];
			} else {
				return defaultValue;
			}
		}

		public static V GetValueOrDefault<U, V>(this IDictionary<U, V> source, U key, V defaultValue = default(V)) {
			return source.SafeGet(key, defaultValue);
		}

		public static V GetRequiredValueOrDefault<U, V>(this IDictionary<U, V> source, U key, V defaultValue = default(V)) {
			if (source.ContainsKey(key)) {
				return source[key];
			}

			Debug.LogError("Failed to find required value for key: " + key);
			return defaultValue;
		}

		public static void SetAndWarnIfReplacing<U, V>(this IDictionary<U, V> source, U key, V value) {
			if (source.ContainsKey(key)) {
				Debug.LogWarning(string.Format("Replacing value for key: {0} with: {1}!", key, value));
			}

			source[key] = value;
		}

		public static V GetOrCreateCached<U, V>(this IDictionary<U, V> source, U key, Func<U, V> valueCreator) {
			if (!source.ContainsKey(key)) {
				source[key] = valueCreator.Invoke(key);
			}
			return source[key];
		}

		public static V GetAndCreateIfNotFound<U, V>(this IDictionary<U, V> source, U key) where V : new() {
			if (!source.ContainsKey(key)) {
				source[key] = new V();
			}
			return source[key];
		}

		public static bool DoesntContainKey<K, V>(this Dictionary<K, V> source, K key) {
			return !source.ContainsKey(key);
		}
	}
}