using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rocks.Dataflow.Fluent;

namespace Rocks.Dataflow.Tests.FluentTests.Infrastructure
{
	[DebuggerDisplay ("{Data}, {Exceptions.Count} exceptions")]
	internal class TestDataflowContext<TData> : IDataflowErrorLogger
	{
		#region Construct

		public TestDataflowContext ()
		{
			this.Exceptions = new List<Exception> ();
		}

		#endregion

		#region Public properties

		public TData Data { get; set; }
		public IList<Exception> Exceptions { get; set; }

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
			lock (this.Exceptions)
				this.Exceptions.Add (exception);
		}

		#endregion
	}
}