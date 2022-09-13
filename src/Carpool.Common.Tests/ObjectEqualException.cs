using Xunit.Sdk;

namespace Carpool.Common.Tests
{
	// Source: https://github.com/nesfit/ICS/blob/71d09b0227bb9b197f98a5c7d9621fda359b50ce/src/CookBook/CookBook.Common.Tests/ObjectEqualException.cs
	public class ObjectEqualException : AssertActualExpectedException
	{
		public ObjectEqualException(object? expected, object? actual, string message)
			: base(expected, actual, "Assert.Equal() Failure")
		{
			Message = message;
		}

		public override string Message { get; }
	}
}
