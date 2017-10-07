#if TMPRO
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using GDataDB;

namespace DTLocalization.Internal {
	public static class TMP_FontAssetUtil {
		// PRAGMA MARK - Public Interface
		public static bool DoesFontContainAllCharacters(TMP_FontAsset font, string text) {
			return text.All(c => GetCharacterSetFor(font).Contains((int)c));
		}


		// PRAGMA MARK - Internal
		private static readonly Dictionary<TMP_FontAsset, HashSet<int>> kFontCharacterSet = new Dictionary<TMP_FontAsset, HashSet<int>>();

		private static HashSet<int> GetCharacterSetFor(TMP_FontAsset font) {
			return kFontCharacterSet.GetOrCreateCached(font, (f) => {
				var visitedFonts = new HashSet<TMP_FontAsset>();
				var charSet = new HashSet<int>();

				var queue = new Queue<TMP_FontAsset>();
				queue.Enqueue(f);

				while (queue.Count > 0) {
					var currentF = queue.Dequeue();
					if (visitedFonts.Contains(currentF)) {
						continue;
					}

					visitedFonts.Add(currentF);
					charSet.UnionWith(currentF.characterDictionary.Keys);
					foreach (var fallbackFont in currentF.fallbackFontAssets) {
						queue.Enqueue(fallbackFont);
					}
				}

				return charSet;
			});
		}
	}
}
#endif
