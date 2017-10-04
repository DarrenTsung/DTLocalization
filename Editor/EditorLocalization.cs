using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

using DTLocalization.Internal;
using Newtonsoft.Json;

namespace DTLocalization {
	// https://msdn.microsoft.com/en-us/library/ee825488%28v=cs.20%29.aspx
	public static class EditorLocalization {
		// PRAGMA MARK - Public Interface
		public static CultureInfo MasterCulture {
			get { return masterCulture_ ?? (masterCulture_ = Convert(EditorPrefs.GetString("EditorLocalization::MasterCulture_", defaultValue: "EN"))); }
		}

		public static CultureInfo[] SupportedCultures {
			get { return supportedCultures_ ?? (supportedCultures_ = JsonConvert.DeserializeObject<string[]>(EditorPrefs.GetString("EditorLocalization::SupportedCultures_")).Select(s => Convert(s)).Where(c => c != null).ToHashSet().ToArray()); }
		}

		public static void SetMasterCulture(string masterCultureString) {
			EditorPrefs.SetString("EditorLocalization::MasterCulture_", masterCultureString);
			masterCulture_ = null;
		}

		public static void SetSupportedCultures(string[] supportedCulturesString) {
			EditorPrefs.SetString("EditorLocalization::SupportedCultures_", JsonConvert.SerializeObject(supportedCulturesString));
			supportedCultures_ = null;
		}


		// PRAGMA MARK - Internal
		private static CultureInfo masterCulture_;
		private static CultureInfo[] supportedCultures_;

		private static CultureInfo Convert(string s) {
			try {
				return new CultureInfo(s);
			} catch (ArgumentException e) {
				Debug.LogWarning("ArgumentException when creating from culture string: " + s + " || error: " + e + " || converting to english!");
				return new CultureInfo("en-US");
			}
		}
	}
}
