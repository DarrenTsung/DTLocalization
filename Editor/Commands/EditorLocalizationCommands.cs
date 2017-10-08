#if DT_COMMAND_PALETTE && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

using DTCommandPalette;

namespace DTLocalization.CommandPaletteCommands {
	public static class EditorLocalizationCommands {
		// PRAGMA MARK - Public Interface
		[MethodCommand]
		public static void SetCurrentLocalizationLanguage() {
			var commandManager = new CommandManager();
			foreach (var supportedCulture in EditorLocalizationConfiguration.GetSupportedCultures()) {
				var chosenCulture = supportedCulture;
				commandManager.AddCommand(new GenericCommand(supportedCulture.DisplayName, () => {
					Localization.SetCurrentCulture(chosenCulture);
				}));
			}

			CommandPaletteWindow.InitializeWindow("Select Current Language", commandManager, clearInput: true);
		}

		[MethodCommand]
		public static void SearchLocalizationKeys() {
			var commandManager = new CommandManager();
			foreach (var localizationTable in LocalizationOfflineCache.LoadAllBundled()) {
				Dictionary<string, string> textMap = localizationTable.GetTextMapFor(EditorLocalizationConfiguration.GetMasterCulture());
				if (textMap == null) {
					Debug.LogWarning("Failed to get TextMap for MasterCulture: " + EditorLocalizationConfiguration.GetMasterCulture().DisplayName);
					return;
				}

				foreach (var kvp in textMap) {
					string key = kvp.Key;
					string localizedText = kvp.Value;

					commandManager.AddCommand(new GenericCommand(localizedText, () => {
						Debug.Log(string.Format("Copied '{0}' into the clipboard!", key));
						EditorGUIUtility.systemCopyBuffer = key;
					}, detailText: key));
				}
			}

			CommandPaletteWindow.InitializeWindow("Localization Keys..", commandManager, clearInput: true);
		}
	}
}
#endif
