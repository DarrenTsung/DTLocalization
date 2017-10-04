using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using UnityEditor;

using DTLocalization.Internal;

namespace DTLocalization.Editor {
	public class EditorLocalizationWindow : EditorWindow {
		// PRAGMA MARK - Static Public Interface
		[MenuItem("Window/DTLocalization Window", priority=100)]
		public static void ShowWindow() {
			EditorWindow.GetWindow<EditorLocalizationWindow>(utility: false, title: "Validator Window");
		}


		// PRAGMA MARK - Public Interface
		public string MasterCulture;
		public string[] SupportedCultures;


		// PRAGMA MARK - Internal
		private SerializedObject target_;

		private void OnEnable() {
			this.MasterCulture = EditorLocalization.MasterCulture.Name;
			this.SupportedCultures = EditorLocalization.SupportedCultures.Select(c => c.Name).ToArray();

			target_ = new SerializedObject(this);
		}

        private void OnGUI() {
            EditorGUILayout.BeginVertical();
				EditorGUI.BeginChangeCheck();

				SerializedProperty masterCultureProperty = target_.FindProperty("MasterCulture");
				EditorGUILayout.PropertyField(masterCultureProperty, includeChildren: true);
				SerializedProperty supportedCulturesProperty = target_.FindProperty("SupportedCultures");
				EditorGUILayout.PropertyField(supportedCulturesProperty, includeChildren: true);

				if (EditorGUI.EndChangeCheck()) {
					EditorLocalization.SetMasterCulture(masterCultureProperty.stringValue);

					string[] supportedCultures = new string[supportedCulturesProperty.arraySize];
					for (int i = 0; i < supportedCulturesProperty.arraySize; i++) {
						supportedCultures[i] = supportedCulturesProperty.GetArrayElementAtIndex(i).stringValue;
					}
					EditorLocalization.SetSupportedCultures(supportedCultures);
				}
			EditorGUILayout.EndVertical();
        }
	}
}
