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

namespace DTLocalization.CommandPaletteCommands {
	public static class DownloadLanguageCharacters {
		// PRAGMA MARK - Public Interface
		[MethodCommand]
		public static void DownloadAllUsedCharacters() {
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

			Directory.CreateDirectory(DownloadPath_);
			string filePath = Path.Combine(DownloadPath_, "AllDownloadedUsedCharacters.txt");
			File.WriteAllText(filePath, charactersString);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log("Downloaded all characters for into txt!");
		}


		// PRAGMA MARK - Internal
		private static readonly string DownloadPath_ = Path.Combine(Application.dataPath, "Localization/DownloadedUsedCharacters");
	}
}
#endif
