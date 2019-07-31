using System;

namespace Rocks.Dataflow.Exceptions
{
	/// <summary>
	///     Dataflow status is invalid for performing current operation.
	/// </summary>
	public class InvalidDataflowStatusException : DataflowException
	{
		private readonly DataflowStatus status;


		public DataflowStatus Status => this.status;


		public InvalidDataflowStatusException (DataflowStatus status, Exception innerException = null)
			: base (string.Format ("Dataflow status is invalid {0} for performing current operation.", status), innerException)
		{
			this.status = status;
		}
	}
}