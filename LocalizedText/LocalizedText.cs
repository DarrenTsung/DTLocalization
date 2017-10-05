using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	public abstract class LocalizedText : MonoBehaviour {
		// PRAGMA MARK - Public Interface
		public void SetKey(string key) {
			key_ = key;
			RefreshLocalizedText();
		}


		// PRAGMA MARK - Internal
		[Header("Properties")]
		[SerializeField]
		private string key_;

		private void OnEnable() {
			RefreshLocalizedText();
			Localization.OnCultureChanged += RefreshLocalizedText;
		}

		private void OnDisable() {
			Localization.OnCultureChanged -= RefreshLocalizedText;
		}

		private void RefreshLocalizedText() {
			string localizedText = Localization.Get(key_);
			SetText(localizedText);
		}

		protected abstract void SetText(string text);
	}
}
