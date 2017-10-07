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
		// PRAGMA MARK - Public Static Interface
		public static Font DowngradedFont {
			get {
				if (downgradedFont_ == null) {
					Debug.LogWarning("downgradedFont_ not found - using first font found in resources (Arial?)! Specify a downgradedFont in your LocalizationConfiguration!");
					downgradedFont_ = Resources.FindObjectsOfTypeAll<Font>()[0];
				}
				return downgradedFont_;
			}
		}

		private static Font downgradedFont_ = null;


		// PRAGMA MARK - Public Interface
		public IEnumerable<ILocalizationTableSource> TableSources {
			get { return gdataTableSources_.Cast<ILocalizationTableSource>(); }
		}


		// PRAGMA MARK - Internal
		[Header("Outlets")]
		[SerializeField]
		private GDatabaseSource[] gdataTableSources_;

		[SerializeField]
		private Font defaultDowngradedFont_;

		private void Awake() {
			if (defaultDowngradedFont_ != null) {
				downgradedFont_ = defaultDowngradedFont_;
			}
		}
	}
}
