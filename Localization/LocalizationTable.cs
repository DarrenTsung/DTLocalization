using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	[Serializable]
	public class LocalizationTable : ISerializationCallbackReceiver {
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
