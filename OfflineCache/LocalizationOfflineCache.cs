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
		#if DT_COMMAND_PALETTE
		[DTCommandPalette.MethodCommand]
		#endif
		public static void CacheLocalizationTables() {
			foreach (var configuration in UnityEngine.Object.FindObjectsOfType<LocalizationConfiguration>()) {
				foreach (var tableSource in configuration.TableSources) {
					var localizationTable = tableSource.LoadTable();
					SaveCached(localizationTable);
				}
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
				File.WriteAllText(filePath, JsonUtility.ToJson(localizationTable));
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			#else
				Debug.LogWarning("Saving cached localization tables is not supported outside of editor!");
			#endif
		}
	}
}
