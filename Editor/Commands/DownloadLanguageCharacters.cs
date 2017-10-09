#if DT_COMMAND_PALETTE && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

using DTCommandPalette;
using TMPro.EditorUtilities;

namespace DTLocalization.CommandPaletteCommands {
	public static class DownloadLanguageCharacters {
		// PRAGMA MARK - Public Interface
		[MethodCommand]
		public static void DownloadAndBakeAllUsedLocalizationCharacters() {
			var charSet = new HashSet<char>();

			LocalizationTable[] allTables = UnityEngine.Object.FindObjectsOfType<LocalizationConfiguration>().SelectMany(config => config.TableSources).Select(s => s.LoadTable()).ToArray();
			foreach (var table in allTables) {
				foreach (var culture in table.AllCultures) {
					var textMap = table.GetTextMapFor(culture);
					foreach (string translatedText in textMap.Values) {
						charSet.UnionWith(translatedText);
					}
				}
			}

			string charactersString = new String(charSet.ToArray());

			var bakedFontConfigurations = AssetDatabaseUtil.AllAssetsOfType<BakedLocalizedFontConfiguration>();
			if (bakedFontConfigurations.Count <= 0) {
				Debug.LogWarning("Cannot bake used localization characters without any BakedLocalizedFontConfigurations in the project!");
				return;
			}

			foreach (var config in bakedFontConfigurations) {
				string outputFilePath = AssetDatabase.GetAssetPath(config.OutputFontAsset);
				TMPFontAssetBaker.Bake(config.Font, config.useAutoSizing, config.FontSize, config.CharacterPadding, config.FontPackingMode, config.AtlasWidth, config.AtlasHeight, config.FontStyle, config.FontStyleMod, config.FontRenderMode, charactersString, outputFilePath);
			}
		}
	}
}
#endif
