using System;

namespace DTLocalization.Internal {
	public static class EnumFlagExtensions {
		public static bool HasFlag(this Enum e, Enum flag) {
			return (Convert.ToInt32(e) & Convert.ToInt32(flag)) > 0;
		}
	}
}
