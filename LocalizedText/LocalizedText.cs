using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if TMPRO
using TMPro;
#endif

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization {
	public class LocalizedText : MonoBehaviour {
		// PRAGMA MARK - Public Interface
		public void SetKey(string key) {
			key_ = key;
			RefreshLocalizedText();
		}

		public void SetUnityText(Text unityText) {
			unityText_ = unityText;
		}

		#if TMPRO
		public void SetTMProText(TMP_Text tmpText) {
			tmpText_ = tmpText;
		}
		#endif


		// PRAGMA MARK - Internal
		[Header("Properties")]
		[SerializeField]
		private string key_ = null;

		[Header("Outlets")]
		[SerializeField]
		#if DT_VALIDATOR
		[DTValidator.Optional]
		#endif
		private Text unityText_;

		#if TMPRO
		[SerializeField]
		#if DT_VALIDATOR
		[DTValidator.Optional]
		#endif
		private TMP_Text tmpText_;
		#endif

		private void OnEnable() {
			RefreshLocalizedText();
			Localization.OnCultureChanged += RefreshLocalizedText;
		}

		private void OnDisable() {
			Localization.OnCultureChanged -= RefreshLocalizedText;
		}

		private void RefreshLocalizedText() {
			if (string.IsNullOrEmpty(key_)) {
				return;
			}

			string localizedText = Localization.Get(key_);
			SetText(localizedText);
		}

		private void SetText(string text) {
			#if TMPRO
			if (tmpText_ != null) {
				tmpText_.text = text;
			}
			#endif

			if (unityText_ != null) {
				unityText_.text = text;
			}
		}
	}
}
