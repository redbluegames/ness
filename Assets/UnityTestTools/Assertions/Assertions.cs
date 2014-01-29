using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityTest
{
	public static class Assertions
	{
		public static void CheckAssertions ()
		{
			var assertions = Object.FindObjectsOfType (typeof (AssertionComponent)) as AssertionComponent[];
			CheckAssertions (assertions);
		}

		public static void CheckAssertions (AssertionComponent assertion)
		{
			CheckAssertions (new[] {assertion});
		}

		public static void CheckAssertions (GameObject gameObject)
		{
			CheckAssertions (gameObject.GetComponents<AssertionComponent> ());
		}

		public static void CheckAssertions (AssertionComponent[] assertions)
		{
			if (!Debug.isDebugBuild)
				return;

			foreach (var assertion in assertions)
			{
				assertion.checksPerformed++;
				var result = assertion.Action.Compare ();
				if (!result)
				{
					assertion.hasFailed = true;
					string message = "";
					if (assertion.Action is ComparerBase)
					{ //needs different message for different comapre to type.
						var comparer = assertion.Action as ComparerBase;
						message = assertion.name + " assertion failed.\n(" + assertion.Action.go + ")." + assertion.Action.thisPropertyPath + " "
							+ comparer.compareToType;

						switch (comparer.compareToType)
						{
								case ComparerBase.CompareToType.CompareToObject:
									message +=" (" + comparer.other + ")." + comparer.otherPropertyPath + " failed.";
									break;
								case ComparerBase.CompareToType.CompareToConstantValue:
									message += comparer.ConstValue + " failed.";
									break;
								case ComparerBase.CompareToType.CompareToNull:
									message += " failed.";
									break;
						}
					}
					else
					{
						message = assertion.name + " assertion failed.\n(" + assertion.Action.go + ")." + assertion.Action.thisPropertyPath + " failed.";
					}

					Debug.LogException (new AssertionException (message), assertion);
				}
			}
		}
	}
}
