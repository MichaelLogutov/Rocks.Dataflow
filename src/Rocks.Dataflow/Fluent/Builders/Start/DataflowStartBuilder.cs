using System;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
	public partial class DataflowStartBuilder<TStart> : IDataflowStartBuilder<TStart>
	{
		/// <summary>
		///     Default exception logger that will be used if currently
		///     processed item in dataflow block does not implements <see cref="IDataflowErrorLogger" />.
		///     If null - no logging will be performed and exception will be swallowed.
		/// </summary>
		[CanBeNull]
		protected internal virtual Action<Exception, object> DefaultExceptionLogger { get; set; }
	}
}