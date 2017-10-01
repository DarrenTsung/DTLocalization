using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GDataDB;

namespace DTLocalization.Internal {
	[Serializable]
	public class GLocalizationRowData {
		public string Key {
			get; set;
		}

		public string LanguageCode {
			get; set;
		}

		public string LocalizedText {
			get; set;
		}
	}
}
