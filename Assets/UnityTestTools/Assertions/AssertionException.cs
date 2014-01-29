using System;

namespace UnityTest
{
	public class AssertionException : Exception
	{
		public AssertionException (string message) : base(message)
		{
		}
	}
}
