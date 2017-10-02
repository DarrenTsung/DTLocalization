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
				CommandPaletteArgumentWindow.Show("Add New Localization Key", (localizationKey) => {
					ITable<GLocalizationMasterRowData> localizationMasterTable = currentDatabaseSource_.LoadLocalizationMasterTable();
					if (localizationMasterTable == null) {
						return;
					}

					var matchingRows = localizationMasterTable.FindStructured(string.Format("Key={0}", localizationKey));
					if (matchingRows.Count > 0) {
						Debug.LogWarning("Found existing row for localization key: " + localizationKey + " cannot adding as new!");
						return;
					}

					var rowData = new GLocalizationMasterRowData();
					rowData.Key = localizationKey;
					localizationMasterTable.Add(rowData);

					// NOTE (darren): we don't delete pre-existing entries in case of data loss
					ITable<GLocalizationRowData> localizationEntryTable = currentDatabaseSource_.LoadLocalizationEntriesTable();
					var entryRows = localizationEntryTable.FindStructured(string.Format("Key={0}", localizationKey));
					if (entryRows.Count > 0) {
						Debug.LogWarning("Found pre-existing rows for localization key: " + localizationKey + ", please verify that they are correct - will not be deleted!");
					}
				});
			});
		}



		// PRAGMA MARK - Internal
		private static GDatabaseSource currentDatabaseSource_;

		private static CultureInfo masterCulture_;
		private static CultureInfo MasterCulture_ {
			get { return masterCulture_ ?? (masterCulture_ = new CultureInfo(EditorPrefs.GetString("GDatabaseCommands::MasterCulture_", defaultValue: "English"))); }
			set { EditorPrefs.SetString("GDatabaseCommands::MasterCulture_", value.Name); masterCulture_ = null; }
		}

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
