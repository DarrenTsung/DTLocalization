using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	public static partial class Localization {
		// PRAGMA MARK - Public Interface
		public static event Action OnCultureChanged = delegate {};
		public static event Action OnLocalizationsUpdated = delegate {};

		public static CultureInfo CurrentCulture {
			get { return currentCulture_; }
		}

		public static void SetCurrentCulture(CultureInfo culture) {
			currentCulture_ = culture;
			OnCultureChanged.Invoke();
		}

		public static CultureInfo GetCachedCultureFor(string cultureString) {
			return cultureMap_.GetOrCreateCached(cultureString, s => new CultureInfo(s));
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


		// PRAGMA MARK - Internal
		private static readonly Dictionary<string, LocalizationTable> localizationTableMap_ = new Dictionary<string, LocalizationTable>();
		private static readonly Dictionary<string, CultureInfo> cultureMap_ = new Dictionary<string, CultureInfo>();

		private static readonly HashSet<string> cachedTableKeys_ = new HashSet<string>();

		private static CultureInfo currentCulture_ = new CultureInfo("EN");

		[RuntimeInitializeOnLoadMethod]
		private static void InitializeLocalization() {
			foreach (var localizationTable in LocalizationOfflineCache.LoadAllCached()) {
				LoadTable(localizationTable);
				cachedTableKeys_.Add(localizationTable.TableKey);
			}

			foreach (var localizationConfiguration in UnityEngine.Object.FindObjectsOfType<LocalizationConfiguration>()) {
				foreach (var tableSource in localizationConfiguration.TableSources) {
					var localizationTable = tableSource.LoadTable();
					if (localizationTable == null) {
						// LoadTable will log reason - we can ignore
						continue;
					}

					LoadTable(localizationTable);
				}
			}
		}

		private static void LoadTable(LocalizationTable localizationTable) {
			string tableKey = localizationTable.TableKey;

			bool cacheOverridden = cachedTableKeys_.Contains(tableKey);
			cachedTableKeys_.Remove(tableKey);

			if (localizationTableMap_.ContainsKey(tableKey) && !cacheOverridden) {
				Debug.LogWarning("LoadTable - overriding non-cached table key: " + tableKey + " table key conflict!");
			}

			localizationTableMap_[tableKey] = localizationTable;
			OnLocalizationsUpdated.Invoke();
		}
	}
}
