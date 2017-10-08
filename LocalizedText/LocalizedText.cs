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

		private Text downgradedText_;
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
				tmpText_.enabled = true;
				tmpText_.text = text;
				if (downgradedText_ != null) {
					downgradedText_.enabled = false;
				}

				// if TMP_Text can't render the text correctly, we must
				// downgrade it to regular Unity font (dynamic rendering)
				if (!TMP_FontAssetUtil.DoesFontContainAllCharacters(tmpText_.font, text)) {
					tmpText_.enabled = false;
					if (downgradedText_ == null) {
						var downgradedTextObj = new GameObject("DowngradedText");
						downgradedTextObj.transform.SetParent(tmpText_.transform, worldPositionStays: false);

						var downgradedRectTransform = downgradedTextObj.AddComponent<RectTransform>();
						downgradedRectTransform.anchorMin = Vector2.zero;
						downgradedRectTransform.anchorMax = Vector2.one;

						downgradedRectTransform.offsetMax = Vector2.zero;
						downgradedRectTransform.offsetMin = Vector2.zero;

						downgradedText_ = downgradedTextObj.AddComponent<Text>();
						downgradedText_.font = LocalizationConfiguration.DowngradedFont;
						tmpText_.CopyPropertiesTo(downgradedText_);

						var contentSizeFitter = tmpText_.GetComponent<ContentSizeFitter>();
						if (contentSizeFitter != null) {
							HorizontalOrVerticalLayoutGroup layoutGroup = tmpText_.gameObject.AddComponent<HorizontalLayoutGroup>();
							layoutGroup.childControlHeight = true;
							layoutGroup.childControlWidth = true;
							layoutGroup.childForceExpandHeight = true;
							layoutGroup.childForceExpandWidth = true;

							var downgradedContentSizeFitter = downgradedText_.gameObject.AddComponent<ContentSizeFitter>();
							downgradedContentSizeFitter.horizontalFit = contentSizeFitter.horizontalFit;
							downgradedContentSizeFitter.verticalFit = contentSizeFitter.verticalFit;
						}
					}

					downgradedText_.text = text;
					downgradedText_.enabled = true;
				}

				return;
			}
			#endif

			if (unityText_ != null) {
				unityText_.text = text;
			}
		}
	}
}
