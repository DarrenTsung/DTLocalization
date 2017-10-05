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

		public static IList<CultureInfo> GetAllCultures() {
			return cultureMap_.Values.ToHashSet().ToArray();
		}

		public static string Get(string key, string localizationTableKey = null) {
			IEnumerable<LocalizationTable> localizationTables = localizationTableMap_.Values;
			if (localizationTableKey != null) {
				if (!localizationTableMap_.ContainsKey(localizationTableKey)) {
					Debug.LogWarning("Get called with invalid localization table key: " + localizationTableKey);
				} else {
					localizationTables = localizationTableMap_[localizationTableKey].Yield();
				}
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
		// Expire cached downloaded bundles
		private const int kExpireCachedDownloadedDays = 1;

		private static readonly Dictionary<string, LocalizationTable> localizationTableMap_ = new Dictionary<string, LocalizationTable>();
		private static readonly Dictionary<string, CultureInfo> cultureMap_ = new Dictionary<string, CultureInfo>();

		private static readonly HashSet<string> cachedTableKeys_ = new HashSet<string>();

		private static CultureInfo currentCulture_ = new CultureInfo("en-US");

		[RuntimeInitializeOnLoadMethod]
		private static void InitializeLocalization() {
			foreach (var localizationTable in LocalizationOfflineCache.LoadAllBundled()) {
				LoadCachedTable(localizationTable);
			}

			Dictionary<string, ILocalizationTableSource> localizationTableSources = new Dictionary<string, ILocalizationTableSource>();
			foreach (var localizationConfiguration in UnityEngine.Object.FindObjectsOfType<LocalizationConfiguration>()) {
				foreach (var tableSource in localizationConfiguration.TableSources) {
					localizationTableSources[tableSource.TableKey] = tableSource;
				}
			}

			HashSet<string> cachedDownloadTableKeys = new HashSet<string>();
			foreach (var cachedLocalizationTable in LocalizationOfflineCache.LoadAllDownloaded()) {
				// override bundled from cached downloaded
				LoadCachedTable(cachedLocalizationTable.LocalizationTable);

				cachedDownloadTableKeys.Add(cachedLocalizationTable.LocalizationTable.TableKey);

				// if cached downloaded data is expired, request new localization table from source
				TimeSpan timePassedSinceCached = DateTime.Now - cachedLocalizationTable.DateTime;
				if (timePassedSinceCached.Days >= kExpireCachedDownloadedDays) {
					DownloadTable(cachedLocalizationTable.LocalizationTable.TableKey, localizationTableSources);
				}
			}

			foreach (var tableKey in localizationTableSources.Keys) {
				if (cachedDownloadTableKeys.Contains(tableKey)) {
					continue;
				}

				DownloadTable(tableKey, localizationTableSources);
			}
		}

		private static void DownloadTable(string tableKey, Dictionary<string, ILocalizationTableSource> localizationTableSources) {
			var tableSource = localizationTableSources.GetRequiredValueOrDefault(tableKey);
			if (tableSource == null) {
				return;
			}

			var localizationTable = tableSource.LoadTable();
			if (localizationTable == null) {
				// Possible network error - we can ignore since LoadTable will log
				return;
			}

			LocalizationOfflineCache.CacheDownloadedTable(localizationTable);
			LoadTable(localizationTable);
		}

		private static void LoadCachedTable(LocalizationTable localizationTable) {
			LoadTable(localizationTable);
			cachedTableKeys_.Add(localizationTable.TableKey);
		}

		private static void LoadTable(LocalizationTable localizationTable) {
			string tableKey = localizationTable.TableKey;

			bool cacheOverridden = cachedTableKeys_.Contains(tableKey);
			cachedTableKeys_.Remove(tableKey);

			if (localizationTableMap_.ContainsKey(tableKey)) {
				if (!cacheOverridden) {
					Debug.LogWarning("LoadTable - merging non-cached table key: " + tableKey + " should not have multiples of the same table key!");
				}

				localizationTableMap_[tableKey].MergeUpdates(localizationTable);
			} else {
				localizationTableMap_[tableKey] = localizationTable;
			}
			OnLocalizationsUpdated.Invoke();
		}
	}
}
