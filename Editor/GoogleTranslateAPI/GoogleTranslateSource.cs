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
	[CreateAssetMenu(fileName = "GoogleTranslateSource", menuName = "DTLocalization/GoogleTranslateSource")]
	public class GoogleTranslateSource : ScriptableObject {
		// PRAGMA MARK - Static Public Interface
		public static GoogleTranslate FindAndCreate() {
			var sources = AssetDatabaseUtil.AllAssetsOfType<GoogleTranslateSource>();
			if (sources.Count <= 0) {
				Debug.LogWarning("No GoogleTranslateSources found in project!");
				return null;
			}

			if (sources.Count > 1) {
				Debug.LogWarning("Multiple GoogleTranslateSources found in project - choosing first!");
			}
			return sources[0].Create();
		}


		// PRAGMA MARK - Public Interface
		public GoogleTranslate Create() {
			return new GoogleTranslate(serviceAccountAddress_, privateKeyP12Asset_.bytes);
		}


		// PRAGMA MARK - Internal
		[Header("OAuth2 Properties")]
		[SerializeField]
		private string serviceAccountAddress_ = "xxxx.gserviceaccount.com";
		[SerializeField]
		private TextAsset privateKeyP12Asset_;
	}
}
