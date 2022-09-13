using KellermanSoftware.CompareNetObjects;

namespace Carpool.Common.Tests
{
	public static class DeepAssert
	{
		// Source: https://github.com/nesfit/ICS/blob/master/src/CookBook/CookBook.Common.Tests/DeepAssert.cs
		// Class for the Assertion Test on Collections, etc.
		public static void Equal<T>(T? expected, T? actual, params string[] propertiesToIgnore)
		{
			CompareLogic compareLogic = new()
			{
				Config =
				{
					MembersToIgnore = new List<string>(),
					IgnoreCollectionOrder = true,
					IgnoreObjectTypes = true,
					CompareStaticProperties = false,
					CompareStaticFields = false
				}
			};

			foreach (var str in propertiesToIgnore)
				compareLogic.Config.MembersToIgnore.Add(str);

			var comparisonResult = compareLogic.Compare((object)expected!, (object)actual!);
			if (!comparisonResult.AreEqual)
				throw new ObjectEqualException((object)expected!, (object)actual!, comparisonResult.DifferencesString);
		}
	}
}
