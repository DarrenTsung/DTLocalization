#if TMPRO
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using GDataDB;

namespace DTLocalization.Internal {
	public static class TMP_TextExtensions {
		// PRAGMA MARK - Public Interface
		public static void CopyPropertiesTo(this TMP_Text tmpText, Text unityText) {
			unityText.fontSize = (int)tmpText.fontSize;
			unityText.alignment = Convert(tmpText.alignment);
		}


		// PRAGMA MARK - Internal
		private static TextAnchor Convert(TextAlignmentOptions alignmentOptions) {
			if (alignmentOptions.HasFlag(_HorizontalAlignmentOptions.Justified) || alignmentOptions.HasFlag(_HorizontalAlignmentOptions.Flush)) {
				Debug.LogWarning("Converting TextAlignmentOptions to TextAnchor - H.Justified / H.Flush not supported, reverting to Left!");
			}

			if (alignmentOptions.HasFlag(_HorizontalAlignmentOptions.Geometry)) {
				Debug.LogWarning("Converting TextAlignmentOptions to TextAnchor - H.Geometry not supported, reverting to Center!");
			}

			if (alignmentOptions.HasFlag(_VerticalAlignmentOptions.Baseline) ||
				alignmentOptions.HasFlag(_VerticalAlignmentOptions.Geometry) ||
				alignmentOptions.HasFlag(_VerticalAlignmentOptions.Capline)) {
				Debug.LogWarning("Converting TextAlignmentOptions to TextAnchor - V.Baseline / V.Geometry / V.Capline not supported, reverting to Middle!");
			}

			switch (alignmentOptions) {
				case TextAlignmentOptions.TopLeft:
				case TextAlignmentOptions.TopJustified:
				case TextAlignmentOptions.TopFlush:
					return TextAnchor.UpperLeft;
				case TextAlignmentOptions.Top:
				case TextAlignmentOptions.TopGeoAligned:
					return TextAnchor.UpperCenter;
				case TextAlignmentOptions.TopRight:
					return TextAnchor.UpperRight;
				case TextAlignmentOptions.Left:
				case TextAlignmentOptions.Justified:
				case TextAlignmentOptions.Flush:
					return TextAnchor.MiddleLeft;
				case TextAlignmentOptions.Center:
				case TextAlignmentOptions.CenterGeoAligned:
					return TextAnchor.MiddleCenter;
				case TextAlignmentOptions.Right:
					return TextAnchor.MiddleRight;
				case TextAlignmentOptions.BottomLeft:
				case TextAlignmentOptions.BottomJustified:
				case TextAlignmentOptions.BottomFlush:
					return TextAnchor.LowerLeft;
				case TextAlignmentOptions.Bottom:
				case TextAlignmentOptions.BottomGeoAligned:
					return TextAnchor.LowerCenter;
				case TextAlignmentOptions.BottomRight:
					return TextAnchor.LowerRight;
				case TextAlignmentOptions.BaselineLeft:
				case TextAlignmentOptions.BaselineJustified:
				case TextAlignmentOptions.BaselineFlush:
					return TextAnchor.UpperLeft;
				case TextAlignmentOptions.Baseline:
				case TextAlignmentOptions.BaselineGeoAligned:
					return TextAnchor.UpperCenter;
				case TextAlignmentOptions.BaselineRight:
					return TextAnchor.UpperRight;
				case TextAlignmentOptions.MidlineLeft:
				case TextAlignmentOptions.MidlineJustified:
				case TextAlignmentOptions.MidlineFlush:
					return TextAnchor.UpperLeft;
				case TextAlignmentOptions.Midline:
				case TextAlignmentOptions.MidlineGeoAligned:
					return TextAnchor.UpperCenter;
				case TextAlignmentOptions.MidlineRight:
					return TextAnchor.UpperRight;
				case TextAlignmentOptions.CaplineLeft:
				case TextAlignmentOptions.CaplineJustified:
				case TextAlignmentOptions.CaplineFlush:
					return TextAnchor.UpperLeft;
				case TextAlignmentOptions.Capline:
				case TextAlignmentOptions.CaplineGeoAligned:
					return TextAnchor.UpperCenter;
				case TextAlignmentOptions.CaplineRight:
					return TextAnchor.UpperRight;
				default:
					Debug.LogWarning("TextAlignmentOptions unrecognized: " + alignmentOptions);
					return TextAnchor.UpperCenter;
			}
		}
	}
}
#endif
