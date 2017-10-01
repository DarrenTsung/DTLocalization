using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

using GDataDB;

namespace DTLocalization.Internal {
	public interface ILocalizationTableSource {
		LocalizationTable LoadTable();
	}
}
