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
	public static class GDatabaseCommands {
		// PRAGMA MARK - Public Interface
		[MethodCommand]
		public static void AddNewLocalizationKey() {
			CheckAndUpdateCurrentDatabaseSource(() => {
				CommandPaletteArgumentWindow.Show("Set Localization Key", (localizationKey) => {
					CultureInfo masterCulture = EditorLocalization.MasterCulture;
					CommandPaletteArgumentWindow.Show(string.Format("Set {0} Text", masterCulture.EnglishName), (masterText) => {
						ITable<GLocalizationMasterRowData> localizationMasterTable = currentDatabaseSource_.LoadLocalizationMasterTable();
						if (localizationMasterTable == null) {
							return;
						}

						ITable<GLocalizationRowData> localizationEntryTable = currentDatabaseSource_.LoadLocalizationEntriesTable();
						if (localizationEntryTable == null) {
							return;
						}

						GoogleTranslate translation = GoogleTranslateSource.FindAndCreate();
						if (translation == null) {
							return;
						}

						bool existingKey = localizationMasterTable.FindAll().Any(r => r.Element.Key == localizationKey);
						if (existingKey) {
							Debug.LogWarning("Found existing row for localization key: " + localizationKey + " cannot adding as new!");
							return;
						}

						var rowData = new GLocalizationMasterRowData();
						rowData.Key = localizationKey;
						localizationMasterTable.Add(rowData);

						// NOTE (darren): we don't delete pre-existing entries in case of data loss
						bool duplicateKey = localizationEntryTable.FindAll().Any(r => r.Element.Key == localizationKey);
						if (duplicateKey) {
							Debug.LogWarning("Found pre-existing rows for localization key: " + localizationKey + ", please verify that they are correct - will not be deleted!");
						}

						foreach (var supportedCulture in EditorLocalization.SupportedCultures) {
							bool isMasterText = supportedCulture.Equals(masterCulture);
							string translatedText;
							if (isMasterText) {
								translatedText = masterText;
							} else {
								translatedText = translation.Translate(masterText, masterCulture.TwoLetterISOLanguageName, supportedCulture.TwoLetterISOLanguageName);
							}

							var entryRowData = new GLocalizationRowData();
							entryRowData.Key = localizationKey;
							entryRowData.LanguageCode = supportedCulture.Name;
							entryRowData.LocalizedText = translatedText;
							entryRowData.SetNeedsUpdating(!isMasterText);
							localizationEntryTable.Add(entryRowData);
						}
					});
				});
			});
		}



		// PRAGMA MARK - Internal
		private static GDatabaseSource currentDatabaseSource_;

		private static void CheckAndUpdateCurrentDatabaseSource(Action callback) {
			// already set
			if (currentDatabaseSource_ != null) {
				callback.Invoke();
				return;
			}

			GDatabaseSource[] allTableSources = UnityEngine.Object.FindObjectsOfType<LocalizationConfiguration>().SelectMany(config => config.TableSources).Where(source => source is GDatabaseSource).Cast<GDatabaseSource>().ToArray();
			if (allTableSources.Length == 0) {
				Debug.LogWarning("No GDatabaseSources found - cannot connect without a database source!");
				return;
			}

			// with only one source - no choosing necessary
			if (allTableSources.Length == 1) {
				currentDatabaseSource_ = allTableSources[0];
				callback.Invoke();
				return;
			}

			// TODO (darren): implement selection between multiple table sources
			Debug.LogWarning("Choosing between multiple table sources not supported yet - selecting first found!");
			currentDatabaseSource_ = allTableSources[0];
			callback.Invoke();
			return;
		}
	}
}
#endif
