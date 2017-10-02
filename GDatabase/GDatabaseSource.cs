using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	[CreateAssetMenu(fileName = "GDatabaseSource", menuName = "DTLocalization/GDatabaseSource")]
	public class GDatabaseSource : ScriptableObject, ILocalizationTableSource {
		// PRAGMA MARK - Public Interface
		public ITable<GLocalizationRowData> LoadLocalizationEntriesTable() {
			try {
				return LoadTableNamed<GLocalizationRowData>(localizationTableName_);
			} catch (WebException e) {
				Debug.LogWarning("Failed to load localization entries table through GDataDB! Exception: " + e);
				return null;
			}
		}

		public ITable<GLocalizationMasterRowData> LoadLocalizationMasterTable() {
			try {
				return LoadTableNamed<GLocalizationMasterRowData>(localizationMasterTableName_);
			} catch (WebException e) {
				Debug.LogWarning("Failed to load localization master table through GDataDB! Exception: " + e);
				return null;
			}
		}


		// PRAGMA MARK - ILocalizationTableSource Implementation
		LocalizationTable ILocalizationTableSource.LoadTable() {
			try {
				var localizationTableData = new Dictionary<CultureInfo, Dictionary<string, string>>();

				var localizationGTable = LoadLocalizationEntriesTable();
				if (localizationGTable == null) {
					return null;
				}

				foreach (var row in localizationGTable.FindAll()) {
					GLocalizationRowData rowData = row.Element;
					CultureInfo culture = Localization.GetCachedCultureFor(rowData.LanguageCode);
					localizationTableData.GetAndCreateIfNotFound(culture)[rowData.Key] = rowData.LocalizedText;
				}

				return new LocalizationTable(localizationTableKey_, localizationTableData);
			} catch (WebException e) {
				Debug.LogWarning("Failed to download latest localization through GDataDB! Exception: " + e);
				return null;
			}
		}


		// PRAGMA MARK - Internal
		[Header("API Properties")]
		[SerializeField]
		private string localizationTableKey_ = "shared";

		[Header("OAuth2 Properties")]
		[SerializeField]
		private string serviceAccountAddress_ = "xxxx.gserviceaccount.com";
		[SerializeField]
		private TextAsset privateKeyP12Asset_;

		[Header("Database Properties")]
		[SerializeField]
		private string databaseName_;
		[SerializeField]
		private string localizationMasterTableName_;
		[SerializeField]
		private string localizationTableName_;

		private byte[] PrivateKey_ { get { return privateKeyP12Asset_.bytes; } }

		private ITable<T> LoadTableNamed<T>(string tableName) where T : new() {
			IDatabaseClient client = new DatabaseClient(serviceAccountAddress_, PrivateKey_);
			IDatabase database = client.GetDatabase(databaseName_);
			if (database == null) {
				Debug.LogWarning("Couldn't find database named: " + databaseName_ + " shared with the service account: " + serviceAccountAddress_ + "!");
				return null;
			}

			ITable<T> table = database.GetTable<T>(tableName);
			if (table == null) {
				Debug.LogWarning("Couldn't find localization table (sheet) named: " + tableName + " in the database named: " + databaseName_ + "!");
				return null;
			}

			return table;
		}
	}
}
