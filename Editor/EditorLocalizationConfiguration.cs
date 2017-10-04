using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

using DTLocalization.Internal;
using Newtonsoft.Json;

namespace DTLocalization {
	// https://msdn.microsoft.com/en-us/library/ee825488%28v=cs.20%29.aspx
	[CreateAssetMenu(fileName = "EditorLocalizationConfiguration", menuName = "DTLocalization/EditorLocalizationConfiguration")]
	public class EditorLocalizationConfiguration : ScriptableObject {
		// PRAGMA MARK - Static
		public static CultureInfo GetMasterCulture() {
			return Config_.MasterCulture_;
		}

		public static CultureInfo[] GetSupportedCultures() {
			return Config_.SupportedCultures_;
		}


		private static EditorLocalizationConfiguration config_;
		private static EditorLocalizationConfiguration Config_ {
			get { return config_ ?? (config_ = AssetDatabaseUtil.GetSingleAssetOfType<EditorLocalizationConfiguration>()); }
		}





		// PRAGMA MARK - Internal
		[Header("Properties")]
		[SerializeField]
		private string serializedMasterCulture_;
		[SerializeField]
		private string[] serializedSupportedCultures_;

		private CultureInfo masterCulture_;
		public CultureInfo MasterCulture_ {
			get { return masterCulture_ ?? (masterCulture_ = Convert(serializedMasterCulture_)); }
		}

		private CultureInfo[] supportedCultures_;
		public CultureInfo[] SupportedCultures_ {
			get { return supportedCultures_ ?? (supportedCultures_ = serializedSupportedCultures_.Select(s => Convert(s)).Where(c => c != null).ToHashSet().ToArray()); }
		}

		private CultureInfo Convert(string s) {
			try {
				return new CultureInfo(s);
			} catch (ArgumentException e) {
				Debug.LogWarning("ArgumentException when creating from culture string: " + s + " || error: " + e + " || converting to english!");
				return new CultureInfo("en-US");
			}
		}
	}
}
