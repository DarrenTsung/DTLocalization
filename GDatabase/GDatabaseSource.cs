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

		public string TableKey {
			get { return localizationTableKey_; }
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
		#pragma warning disable 0414
		[SerializeField]
		private string serviceAccountAddress_ = "xxxx.gserviceaccount.com";
		[SerializeField]
		private TextAsset privateKeyP12Asset_;
		#pragma warning restore 0414

		#if UNITY_EDITOR
		[Header("Editor OAuth2 Properties (write-access)")]
		[SerializeField]
		private string editorServiceAccountAddress_ = "xxxx.gserviceaccount.com";
		[SerializeField]
		private TextAsset editorPrivateKeyP12Asset_;
		#endif

		[Header("Database Properties")]
		[SerializeField]
		private string databaseName_;
		[SerializeField]
		private string localizationMasterTableName_;
		[SerializeField]
		private string localizationTableName_;

		private ITable<T> LoadTableNamed<T>(string tableName) where T : new() {
			string serviceAccount;
			byte[] privateKey;

			#if UNITY_EDITOR
			serviceAccount = editorServiceAccountAddress_;
			privateKey = editorPrivateKeyP12Asset_.bytes;
			#else
			serviceAccount = serviceAccountAddress_;
			privateKey = privateKeyP12Asset_.bytes;
			#endif

			IDatabaseClient client = new DatabaseClient(serviceAccount, privateKey);

			IDatabase database = client.GetDatabase(databaseName_);
			if (database == null) {
				Debug.LogWarning("Couldn't find database named: " + databaseName_ + " shared with the service account: " + serviceAccount + "!");
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
