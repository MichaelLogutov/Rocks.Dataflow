using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Rocks.Dataflow.Fluent;

namespace Rocks.Dataflow.Tests.FluentTests.Infrastructure
{
	[DebuggerDisplay ("{Data}, {Exceptions.Count} exceptions")]
	internal class TestDataflowContext<TData> : IDataflowErrorLogger
	{
		#region Private fields

		private readonly ConcurrentQueue<Exception> exceptions;

		#endregion

		#region Construct

		public TestDataflowContext ()
		{
			this.exceptions = new ConcurrentQueue<Exception> ();
		}

		#endregion

		#region Public properties

		public TData Data { get; set; }
		public ConcurrentQueue<Exception> Exceptions { get { return this.exceptions; } }

		#endregion

		#region Public methods

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		///     A string that represents the current object.
		/// </returns>
		public override string ToString ()
		{
			return string.Format ("{0}", this.Data);
		}

		#endregion

		#region IDataflowErrorLogger Members

		/// <summary>
		///     Called when one of the execution dataflow block faulted with the <paramref name="exception" />.
		/// </summary>
		void IDataflowErrorLogger.OnException (Exception exception)
		{
			this.Exceptions.Enqueue (exception);
		}

		#endregion
	}
}