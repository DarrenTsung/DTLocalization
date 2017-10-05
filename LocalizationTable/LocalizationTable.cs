using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization.Internal {
	[Serializable]
	public class LocalizationTable : ISerializationCallbackReceiver {
		// PRAGMA MARK - Public Interface
		public string TableKey { get { return tableKey_; } }

		public IEnumerable<CultureInfo> AllCultures {
			get { return cultureKeyLocalizedTextMap_.Keys; }
		}

		public Dictionary<string, string> GetTextMapFor(CultureInfo cultureInfo) {
			return cultureKeyLocalizedTextMap_.GetValueOrDefault(cultureInfo);
		}

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

		// NOTE (darren): Merge updates from more recent tables without losing data
		// For example bundled localization has key BUTTON_TARGET_PHRASE, but downloaded does not
		// Merge the downloaded on top of the bundled and keep BUTTON_TARGET_PHRASE
		// but update the localized text for the keys that downloaded has
		//
		// Basically prevent data loss if it was included in some earlier version of LocalizationTable (bundled)
		public void MergeUpdates(LocalizationTable updatedLocalizationTable) {
			if (!updatedLocalizationTable.TableKey.Equals(TableKey)) {
				Debug.LogWarning("Attempting to merge updates for a localization table that does not match table key! Ignoring.");
				return;
			}

			foreach (var kvp in updatedLocalizationTable.cultureKeyLocalizedTextMap_) {
				var cultureInfo = kvp.Key;
				var updatedTextMap = kvp.Value;

				if (!cultureKeyLocalizedTextMap_.ContainsKey(cultureInfo)) {
					cultureKeyLocalizedTextMap_[cultureInfo] = updatedTextMap;
				} else {
					var textMap = cultureKeyLocalizedTextMap_[cultureInfo];
					foreach (string key in updatedTextMap.Keys) {
						string updatedValue = updatedTextMap[key];
						textMap[key] = updatedValue;
					}
				}
			}
		}


		// PRAGMA MARK - ISerializationCallbackReceiver Implementation
		void ISerializationCallbackReceiver.OnAfterDeserialize() {
			cultureKeyLocalizedTextMap_ = cultureTables_.ToMap(t => t.UnpackCulture(), t => t.UnpackTextMap());
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize() {
			cultureTables_ = cultureKeyLocalizedTextMap_.Select(kvp => new SerializedLocalizationCultureTable(kvp.Key, kvp.Value)).ToArray();
		}


		// PRAGMA MARK - Internal
		[Header("Properties")]
		[SerializeField]
		private string tableKey_;
		[SerializeField]
		private SerializedLocalizationCultureTable[] cultureTables_;

		private Dictionary<CultureInfo, Dictionary<string, string>> cultureKeyLocalizedTextMap_;

		[Serializable]
		private class SerializedLocalizationCultureTable {
			// PRAGMA MARK - Public Interface
			public CultureInfo UnpackCulture() {
				return Localization.GetCachedCultureFor(cultureName_);
			}

			public Dictionary<string, string> UnpackTextMap() {
				return keyTextTuples_.ToMap(t => t.Item1, t => t.Item2);
			}

			public SerializedLocalizationCultureTable(CultureInfo culture, Dictionary<string, string> textMap) {
				cultureName_ = culture.Name;
				keyTextTuples_ = textMap.Select(kvp => new StringStringTuple(kvp.Key, kvp.Value)).ToArray();
			}


			// PRAGMA MARK - Internal
			[Header("Properties")]
			[SerializeField]
			private string cultureName_;
			[SerializeField]
			private StringStringTuple[] keyTextTuples_;
		}

		[Serializable]
		private class StringStringTuple {
			public string Item1;
			public string Item2;

			public StringStringTuple(string item1, string item2) {
				Item1 = item1;
				Item2 = item2;
			}
		}
	}
}
