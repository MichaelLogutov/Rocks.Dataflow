using System;

namespace Rocks.Dataflow.Exceptions
{
	/// <summary>
	///     An error occured while processing dataflow.
	/// </summary>
	public class DataflowException : Exception
	{
		public DataflowException (Exception innerException = null)
			: base ("An error occured while processing dataflow.", innerException)
		{
		}


		public DataflowException (string message, Exception innerException = null)
			: base (message, innerException)
		{
		}
	}
}