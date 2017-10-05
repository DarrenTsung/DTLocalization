using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

namespace DTLocalization.Internal {
	public class LocalizationAutoSwitch : MonoBehaviour {
		// PRAGMA MARK - Public Interface
		public static bool Enabled {
			get { return enabled_; }
			set {
				if (!Application.isPlaying) {
					Debug.LogWarning("LocalizationAutoSwitch - cannot change Enabled when not playing!");
					return;
				}

				if (enabled_ == value) {
					return;
				}

				enabled_ = value;
				if (enabled_) {
					autoSwitchCoroutine_ = MonoBehaviour_.StartCoroutine(AutoSwitchCoroutine());
				} else {
					if (autoSwitchCoroutine_ != null) {
						MonoBehaviour_.StopCoroutine(autoSwitchCoroutine_);
						autoSwitchCoroutine_ = null;
					}
				}
			}
		}


		// PRAGMA MARK - Internal
		private static Coroutine autoSwitchCoroutine_;
		private static bool enabled_ = false;

		private static GameObject gameObject_;
		private static GameObject GameObject_ {
			get { return gameObject_ ?? (gameObject_ = new GameObject("LocalizationAutoSwitch")); }
		}

		private static LocalizationAutoSwitch monoBehaviour_;
		private static LocalizationAutoSwitch MonoBehaviour_ {
			get { return monoBehaviour_ ?? (monoBehaviour_ = GameObject_.AddComponent<LocalizationAutoSwitch>()); }
		}


		private static IEnumerator AutoSwitchCoroutine() {
			var waitForSeconds = new WaitForSeconds(0.2f);

			var allCultures = Localization.GetAllCultures();
			int index = 0;
			while (true) {
				Localization.SetCurrentCulture(allCultures[index]);
				yield return waitForSeconds;
				index = (index + 1) % allCultures.Count;
			}
		}
	}
}
