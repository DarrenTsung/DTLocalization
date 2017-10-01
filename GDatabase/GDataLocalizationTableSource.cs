using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GDataDB;

namespace DTLocalization {
	[CreateAssetMenu(fileName = "GDataLocalizationTableSource", menuName = "DTLocalization/GDataLocalizationTableSource")]
	public class GDataLocalizationTableSource : ScriptableObject {
		// PRAGMA MARK - Public Interface
		public string LocalizationTableKey { get { return localizationTableKey_; } }

		public ITable<GLocalizationRowData> LoadLocalizationTable() {
			IDatabaseClient client = new DatabaseClient(serviceAccountAddress_, PrivateKey_);
			IDatabase database = client.GetDatabase(databaseName_);
			if (database == null) {
				Debug.LogWarning("Couldn't find database named: " + databaseName_ + " shared with the service account: " + serviceAccountAddress_ + "!");
				return null;
			}

			ITable<GLocalizationRowData> table = database.GetTable<GLocalizationRowData>(localizationTableName_);
			if (table == null) {
				Debug.LogWarning("Couldn't find localization table (sheet) named: " + localizationTableName_ + " in the database named: " + databaseName_ + "!");
				return null;
			}

			return table;
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
		private string localizationTableName_;

		private byte[] PrivateKey_ { get { return privateKeyP12Asset_.bytes; } }
	}
}
