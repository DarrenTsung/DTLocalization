using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using DTLocalization.Internal;

#if TMPRO
using TMPro;
#endif

namespace DTLocalization {
	// To be used by adding to prefabs (non dynamic text)
	public class SerializedLocalizedText : LocalizedText {
		// PRAGMA MARK - Internal
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

		protected override void SetText(string text) {
			if (unityText_ != null) {
				unityText_.text = text;
			}

			#if TMPRO
			if (tmpText_ != null) {
				tmpText_.text = text;
			}
			#endif
		}
	}
}
