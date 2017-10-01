using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	public class LocalizationTable {
		// PRAGMA MARK - Public Interface
		public string TableKey { get { return tableKey_; } }

		public LocalizationTable(string tableKey, Dictionary<CultureInfo, Dictionary<string, string>> cultureKeyLocalizedTextMap) {
			tableKey_ = tableKey;
			cultureKeyLocalizedTextMap_ = cultureKeyLocalizedTextMap;
		}

		public string Get(CultureInfo cultureInfo, string key) {
			var keyLocalizedTextMap = cultureKeyLocalizedTextMap_.GetValueOrDefault(cultureInfo);
			if (keyLocalizedTextMap == null) {
				return null;
			}

			return keyLocalizedTextMap.GetValueOrDefault(key);
		}


		// PRAGMA MARK - Internal
		private readonly Dictionary<CultureInfo, Dictionary<string, string>> cultureKeyLocalizedTextMap_;
		private string tableKey_;
	}
}
