#if TMPRO && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

using DTLocalization.Internal;
using GDataDB;

using DTCommandPalette;
using TMPro;
using TMPro.EditorUtilities;

namespace DTLocalization.Internal {
	[CreateAssetMenu(fileName = "BakedLocalizedFontConfiguration", menuName = "DTLocalization/BakedLocalizedFontConfiguration")]
	public class BakedLocalizedFontConfiguration : ScriptableObject {
		public Font Font;

		[Space]
		public bool useAutoSizing = true;
		// Not necessary if useAutoSizing
		public int FontSize = 60;
		public int CharacterPadding = 5;

		[Space]
		public TMPFontPackingModes FontPackingMode;

		[Space]
		public int AtlasWidth = 512;
		public int AtlasHeight = 512;

		[Space]
		public FaceStyles FontStyle;
		public int FontStyleMod = 2;

		[Space]
		public RenderModes FontRenderMode;

		[Space]
		public TMP_FontAsset OutputFontAsset;
	}
}
#endif
