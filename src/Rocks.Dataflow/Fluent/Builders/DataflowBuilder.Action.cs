using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Action;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract partial class DataflowBuilder<TBuilder, TStart, TInput, TOutput>
	{
		/// <summary>
		///     Ends the dataflow with the <see cref="ActionBlock{TOutput}" /> using <paramref name="process" /> as a body.
		/// </summary>
		public DataflowActionBuilder<TStart, TOutput> ActionAsync (Func<TOutput, Task> process)
		{
			return new DataflowActionBuilder<TStart, TOutput> (this, process);
		}


		/// <summary>
		///     Ends the dataflow with the <see cref="ActionBlock{TOutput}" /> using <paramref name="process" /> as a body.
		/// </summary>
		public DataflowActionBuilder<TStart, TOutput> Action (Action<TOutput> process)
		{
			return new DataflowActionBuilder<TStart, TOutput> (this, process);
		}
	}
}