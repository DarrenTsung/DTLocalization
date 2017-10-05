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
		[Serializable]
		public class CachedLocalizationTable : ISerializationCallbackReceiver {
			public LocalizationTable LocalizationTable;
			public DateTime DateTime;


			// PRAGMA MARK - ISerializationCallbackReceiver Implementation
			void ISerializationCallbackReceiver.OnAfterDeserialize() {
				DateTime = new DateTime(serializedDateTime_);
			}

			void ISerializationCallbackReceiver.OnBeforeSerialize() {
				serializedDateTime_ = DateTime.Ticks;
			}


			// PRAGMA MARK - Internal
			[SerializeField]
			private long serializedDateTime_;
		}

		public static LocalizationTable[] LoadAllBundled() {
			return Resources.LoadAll<TextAsset>("LocalizationOfflineCache").Select(serialized => JsonUtility.FromJson<LocalizationTable>(serialized.text)).ToArray();
		}

		public static IList<CachedLocalizationTable> LoadAllDownloaded() {
			List<CachedLocalizationTable> cachedTables = new List<CachedLocalizationTable>();

			string cachePath = GetPersistentCachePath();
			foreach (string filePath in Directory.GetFiles(cachePath)) {
				using (BinaryReader b = new BinaryReader(System.IO.File.Open(filePath, FileMode.Open))) {
					cachedTables.Add(JsonUtility.FromJson<CachedLocalizationTable>(b.ReadString()));
				}
			}

			return cachedTables;
		}

		public static void CacheDownloadedTable(LocalizationTable localizationTable) {
			string cachePath = GetPersistentCachePath();
			string filePath = Path.Combine(cachePath, localizationTable.TableKey + ".txt");

			var cachedTable = new CachedLocalizationTable();
			cachedTable.LocalizationTable = localizationTable;
			cachedTable.DateTime = DateTime.Now;

			using (BinaryWriter b = new BinaryWriter(System.IO.File.Open(filePath, FileMode.Create))) {
				b.Write(JsonUtility.ToJson(cachedTable));
			}
		}

		// NOTE (darren): call this from your build script / CI environment
		#if UNITY_EDITOR
		#if DT_COMMAND_PALETTE
		[DTCommandPalette.MethodCommand]
		#endif
		public static void CacheBundledLocalizationTables() {
			foreach (var configuration in UnityEngine.Object.FindObjectsOfType<LocalizationConfiguration>()) {
				foreach (var tableSource in configuration.TableSources) {
					var localizationTable = tableSource.LoadTable();
					CacheBundledTable(localizationTable);
				}
			}
		}
		#endif


		// PRAGMA MARK - Internal
		private static string OfflineCachePath_ { get { return Path.Combine(Application.dataPath, "Resources/LocalizationOfflineCache"); } }

		private static string GetPersistentCachePath() {
			string cachePath = Path.Combine(Application.persistentDataPath, "LocalizationOfflineCache");
			if (!Directory.Exists(cachePath)) {
				Directory.CreateDirectory(cachePath);
			}

			return cachePath;
		}

		#if UNITY_EDITOR
		private static void CacheBundledTable(LocalizationTable localizationTable) {
				if (localizationTable == null) {
					return;
				}

				string offlineCachePath = OfflineCachePath_;
				Directory.CreateDirectory(offlineCachePath);

				string filePath = Path.Combine(offlineCachePath, string.Format("{0}.txt", localizationTable.TableKey));
				File.WriteAllText(filePath, JsonUtility.ToJson(localizationTable));
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
		}
		#endif
	}
}
