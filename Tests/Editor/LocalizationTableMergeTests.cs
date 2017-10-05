using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

using NUnit.Framework;

namespace DTLocalization.Internal.Tests {
	public static class LocalizationTableMergeTests {
		[Test]
		public static void MergeUpdate_WorksAsExpected() {
			var bundledTable = new LocalizationTable("Generic", new Dictionary<CultureInfo, Dictionary<string, string>>()
			{
				{ new CultureInfo("en"), new Dictionary<string, string>()
					{
						{ "GENERIC_YES", "Yes" },
						{ "GENERIC_NO", "No" }
					}
				}
			});

			var updatedTable = new LocalizationTable("Generic", new Dictionary<CultureInfo, Dictionary<string, string>>()
			{
				{ new CultureInfo("en"), new Dictionary<string, string>()
					{
						{ "GENERIC_NO", "Nien" }
					}
				}
			});

			bundledTable.MergeUpdates(updatedTable);

			Assert.That(bundledTable.Get(new CultureInfo("en"), "GENERIC_NO"), Is.EqualTo("Nien"));
			Assert.That(bundledTable.Get(new CultureInfo("en"), "GENERIC_YES"), Is.EqualTo("Yes"));
		}

		[Test]
		public static void MergeUpdate_WithIncorrectKeys_WorksAsExpected() {
			var bundledTable = new LocalizationTable("Generic", new Dictionary<CultureInfo, Dictionary<string, string>>()
			{
				{ new CultureInfo("en"), new Dictionary<string, string>()
					{
						{ "GENERIC_YES", "Yes" },
						{ "GENERIC_NO", "No" }
					}
				}
			});

			var updatedTable = new LocalizationTable("NotGeneric", new Dictionary<CultureInfo, Dictionary<string, string>>()
			{
				{ new CultureInfo("en"), new Dictionary<string, string>()
					{
						{ "GENERIC_NO", "Nien" }
					}
				}
			});

			Debug.logger.logEnabled = false;
			bundledTable.MergeUpdates(updatedTable);
			Debug.logger.logEnabled = true;

			Assert.That(bundledTable.Get(new CultureInfo("en"), "GENERIC_NO"), Is.EqualTo("No"));
			Assert.That(bundledTable.Get(new CultureInfo("en"), "GENERIC_YES"), Is.EqualTo("Yes"));
		}
	}
}