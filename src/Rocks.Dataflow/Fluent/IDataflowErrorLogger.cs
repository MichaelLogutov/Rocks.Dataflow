using System;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent
{
	/// <summary>
	///     Represents a class that can be notified about errors inside dataflow blocks.
	/// </summary>
	public interface IDataflowErrorLogger
	{
		/// <summary>
		///     Called when one of the execution dataflow block faulted with the <paramref name="exception" />.
		/// </summary>
		void OnException ([NotNull] Exception exception);
	}
}
