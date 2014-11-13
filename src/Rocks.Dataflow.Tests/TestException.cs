using System;

namespace Rocks.Dataflow.Tests
{
	/// <summary>
	///     Test exception.
	/// </summary>
	public class TestException : Exception
	{
		public TestException (Exception innerException = null)
			: base ("Test exception.", innerException)
		{
		}


		public TestException (string message, Exception innerException = null)
			: base (message, innerException)
		{
		}
	}
}