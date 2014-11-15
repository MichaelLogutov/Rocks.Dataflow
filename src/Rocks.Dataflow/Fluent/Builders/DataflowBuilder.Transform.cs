using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Tranform;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract partial class DataflowBuilder<TBuilder, TStart, TInput, TOutput>
	{
		/// <summary>
		///     Continues the dataflow with the <see cref="TransformBlock{TOutput,TNewOutput}" /> using <paramref name="process" />
		///     as a body.
		/// </summary>
		public DataflowTranformBuilder<TStart, TOutput, TNewOutput> TransformAsync<TNewOutput> (Func<TOutput, Task<TNewOutput>> process)
		{
			return new DataflowTranformBuilder<TStart, TOutput, TNewOutput> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the <see cref="TransformBlock{TOutput,TNewOutput}" /> using <paramref name="process" />
		///     as a body.
		/// </summary>
		public DataflowTranformBuilder<TStart, TOutput, TNewOutput> Transform<TNewOutput> (Func<TOutput, TNewOutput> process)
		{
			return new DataflowTranformBuilder<TStart, TOutput, TNewOutput> (this, process);
		}
	}
}