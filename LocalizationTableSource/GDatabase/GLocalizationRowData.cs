using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GDataDB;

namespace DTLocalization.Internal {
	[Serializable]
	public class GLocalizationRowData {
		public string Key {
			get; set;
		}

		public string LanguageCode {
			get; set;
		}

		public string LocalizedText {
			get; set;
		}

		// TRUE or FALSE
		public string NeedsUpdating {
			get; set;
		}
	}

	public static class GLocalizationRowDataExtensions {
		public static bool GetNeedsUpdating(this GLocalizationRowData data) {
			return data.NeedsUpdating == "TRUE";
		}

		public static void SetNeedsUpdating(this GLocalizationRowData data, bool needsUpdating) {
			data.NeedsUpdating = needsUpdating ? "TRUE" : "FALSE";
		}
	}
}
