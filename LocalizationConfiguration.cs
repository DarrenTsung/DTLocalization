using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	public class LocalizationConfiguration : MonoBehaviour {
		// PRAGMA MARK - Internal
		[Header("Outlets")]
		[SerializeField]
		private GDataLocalizationTableSource[] gdataTableSources_;

		private void Awake() {
			foreach (var tableSource in gdataTableSources_) {
				Localization.LoadTableSource(tableSource);
			}
		}
	}
}
