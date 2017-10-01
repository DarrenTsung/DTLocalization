using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	public static class Localization {
		// PRAGMA MARK - Public Interface
		public static event Action OnCultureChanged = delegate {};

		public static CultureInfo CurrentCulture {
			get { return currentCulture_; }
		}

		public static void SetCurrentCulture(CultureInfo culture) {
			currentCulture_ = culture;
			OnCultureChanged.Invoke();
		}

		public static string Get(string key, string localizationTableKey = null) {
			IEnumerable<LocalizationTable> localizationTables = localizationTableMap_.Values;
			if (localizationTableKey != null) {
				localizationTables = localizationTableMap_.GetRequiredValueOrDefault(localizationTableKey).Yield();
			}

			string localizedText = null;
			foreach (var localizationTable in localizationTables) {
				string result = localizationTable.Get(currentCulture_, key);
				if (result == null) {
					continue;
				}

				// NOTE (darren): found a result - if this is the second result
				// then log a warning for the table
				if (localizedText != null) {
					Debug.LogWarning("Found multiple entries for key: " + key + " in table: " + localizationTable.TableKey);
				}
				localizedText = result;
			}

			if (localizedText == null) {
				Debug.LogWarning("Get - Failed to find localized text for key: " + key + " | localizationTableKey: " + localizationTableKey);
			}
			return localizedText;
		}

		public static void LoadTableSource(GDataLocalizationTableSource tableSource) {
			var localizationTableData = new Dictionary<CultureInfo, Dictionary<string, string>>();

			var localizationGTable = tableSource.LoadLocalizationTable();
			foreach (var row in localizationGTable.FindAll()) {
				GLocalizationRowData rowData = row.Element;
				CultureInfo culture = cultureMap_.GetOrCreateCached(rowData.LanguageCode, lc => new CultureInfo(lc));
				localizationTableData.GetAndCreateIfNotFound(culture)[rowData.Key] = rowData.LocalizedText;
			}

			string tableKey = tableSource.LocalizationTableKey;
			localizationTableMap_[tableKey] = new LocalizationTable(tableKey, localizationTableData);
		}


		// PRAGMA MARK - Internal
		private static readonly Dictionary<string, LocalizationTable> localizationTableMap_ = new Dictionary<string, LocalizationTable>();
		private static readonly Dictionary<string, CultureInfo> cultureMap_ = new Dictionary<string, CultureInfo>();

		private static CultureInfo currentCulture_ = new CultureInfo("EN");
	}
}
