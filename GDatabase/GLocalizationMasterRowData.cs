using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using GDataDB;

namespace DTLocalization.Internal {
	[Serializable]
	public class GLocalizationMasterRowData {
		public string Key {
			get; set;
		}

		public string Context {
			get; set;
		}
	}
}
