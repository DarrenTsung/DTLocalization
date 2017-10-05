using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

#if DT_DEBUG_MENU
using DTDebugMenu;
#endif

namespace DTLocalization {
	public static partial class Localization {
		#if DT_DEBUG_MENU
		// PRAGMA MARK - Internal
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeDebugMenu() {
			RefreshDebugMenu();
			OnLocalizationsUpdated += RefreshDebugMenu;
			OnCultureChanged += RefreshDebugMenu;
		}

		private static void RefreshDebugMenu() {
			var inspector = DTDebugMenu.GenericInspectorRegistry.Get("Localization");
			inspector.ResetFields();

			inspector.RegisterHeader("Properties:");
			inspector.RegisterLabel(string.Format("Current Language: {0}", CurrentCulture.DisplayName));

			inspector.RegisterHeader("Debug Tools:");
			inspector.RegisterToggle("Auto-Switch Languages", () => LocalizationAutoSwitch.Enabled, (b) => LocalizationAutoSwitch.Enabled = b);
			inspector.RegisterButton("Previous Language", () => MoveCultureIndex(i => i - 1));
			inspector.RegisterButton("Next Language", () => MoveCultureIndex(i => i + 1));

			inspector.RegisterHeader("Localization Tables:");
			foreach (var kvp in localizationTableMap_) {
				string tableKey = kvp.Key;
				LocalizationTable table = kvp.Value;

				bool fromCached = cachedTableKeys_.Contains(tableKey);
				inspector.RegisterLabel(string.Format("{0}{1}", tableKey, fromCached ? " (cached)" : ""));
			}
		}

		private static void MoveCultureIndex(Func<int, int> movementTransformer) {
			if (LocalizationAutoSwitch.Enabled) {
				LocalizationAutoSwitch.Enabled = false;
			}

			var allCultures = Localization.AllCultures;
			int index = allCultures.IndexOf(Localization.CurrentCulture);
			index = movementTransformer.Invoke(index);

			Localization.SetCurrentCulture(allCultures[(allCultures as IList).WrapIndex(index)]);
		}
		#endif
	}
}
