using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	// To be used by adding through code
	public class RuntimeLocalizedText : LocalizedText {
		// PRAGMA MARK - Public Interface
		public void Init(string key, Action<string> setTextCallback) {
			setTextCallback_ = setTextCallback;
			SetKey(key);
		}


		// PRAGMA MARK - Internal
		private Action<string> setTextCallback_;

		protected override void SetText(string text) {
			if (setTextCallback_ != null) {
				Debug.LogWarning("setTextCallback_ is missing - call Init on RuntimeLocalizedText!");
			}

			setTextCallback_.Invoke(text);
		}
	}
}
