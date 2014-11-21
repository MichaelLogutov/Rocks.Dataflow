using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Tranform;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract partial class DataflowBuilder<TBuilder, TStart, TInput, TOutput>
	{
		/// <summary>
		///     Continues the dataflow with the <see cref="TransformBlock{TOutput,TInput}" /> using <paramref name="process" />
		///     as a body.
		/// </summary>
		public DataflowProcessBuilder<TStart, TOutput> ProcessAsync (Func<TOutput, Task> process)
		{
			return new DataflowProcessBuilder<TStart, TOutput> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the <see cref="TransformBlock{TOutput,TNewOutput}" /> using <paramref name="process" />
		///     as a body.
		/// </summary>
		public DataflowProcessBuilder<TStart, TOutput> Process (Action<TOutput> process)
		{
			return new DataflowProcessBuilder<TStart, TOutput> (this, process);
		}
	}
}