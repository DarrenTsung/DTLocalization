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
			if (Application.internetReachability == NetworkReachability.NotReachable) {
				Debug.LogWarning("No internet connection - cannot add new localization key!");
				return;
			}

			CheckAndUpdateCurrentDatabaseSource(() => {
				CommandPaletteArgumentWindow.Show("Set Localization Key", (localizationKey) => {
					CultureInfo masterCulture = EditorLocalizationConfiguration.GetMasterCulture();
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

						foreach (var supportedCulture in EditorLocalizationConfiguration.GetSupportedCultures()) {
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

			// choose between database sources
			var commandManager = new CommandManager();
			foreach (var databaseSource in allTableSources) {
				var chosenDatabaseSource = databaseSource;
				commandManager.AddCommand(new GenericCommand(databaseSource.TableKey, () => {
					currentDatabaseSource_ = chosenDatabaseSource;
					callback.Invoke();
				}));
			}

			CommandPaletteWindow.InitializeWindow("Select Database To Use", commandManager, clearInput: true);
		}
	}
}
#endif
