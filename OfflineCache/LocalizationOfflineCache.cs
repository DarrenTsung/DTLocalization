using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	public static class LocalizationOfflineCache {
		// PRAGMA MARK - Public Interface
		public static LocalizationTable[] LoadAllCached() {
			return Resources.LoadAll<TextAsset>("LocalizationOfflineCache").Select(serialized => JsonUtility.FromJson<LocalizationTable>(serialized.text)).ToArray();
		}

		// NOTE (darren): call this from your build script / CI environment
		public static void SaveTablesToCache() {
			foreach (var configuration in UnityEngine.Object.FindObjectsOfType<LocalizationConfiguration>()) {
				foreach (var tableSource in configuration.TableSources) {
					var localizationTable = tableSource.LoadTable();
					SaveCached(localizationTable);
				}
			}
		}

		public static void ClearCachedTablesFromLocalBuild() {
			string offlineCachePath = OfflineCachePath_;
			Directory.Delete(offlineCachePath, recursive: true);

			string offlineCachePathMeta = offlineCachePath + ".meta";
			if (File.Exists(offlineCachePathMeta)) {
				File.Delete(offlineCachePathMeta);
			}
		}


		// PRAGMA MARK - Internal
		private static string OfflineCachePath_ { get { return Path.Combine(Application.dataPath, "Resources/LocalizationOfflineCache"); } }

		private static void SaveCached(LocalizationTable localizationTable) {
			#if UNITY_EDITOR
				if (localizationTable == null) {
					return;
				}

				string offlineCachePath = OfflineCachePath_;
				Directory.CreateDirectory(offlineCachePath);

				string filePath = Path.Combine(offlineCachePath, string.Format("{0}.txt", localizationTable.TableKey));
				if (File.Exists(filePath)) {
					Debug.LogWarning("Replacing cached localization table at: " + filePath + ", should not have previously built caches - possible overlap with table keys!");
				}

				File.WriteAllText(filePath, JsonUtility.ToJson(localizationTable));
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			#else
				Debug.LogWarning("Saving cached localization tables is not supported outside of editor!");
			#endif
		}
	}
}
