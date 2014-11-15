using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Tranform;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract partial class DataflowBuilder<TBuilder, TStart, TInput, TOutput>
	{
		/// <summary>
		///     Continues the dataflow with the <see cref="TransformManyBlock{TOutput,TNewOutput}" /> using
		///     <paramref name="process" /> as a body.
		/// </summary>
		public DataflowTranformManyBuilder<TStart, TOutput, TNewOutput> TransformManyAsync<TNewOutput> (Func<TOutput, Task<IEnumerable<TNewOutput>>> process)
		{
			return new DataflowTranformManyBuilder<TStart, TOutput, TNewOutput> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the <see cref="TransformManyBlock{TOutput,TNewOutput}" /> using
		///     <paramref name="process" /> as a body.
		/// </summary>
		public DataflowTranformManyBuilder<TStart, TOutput, TNewOutput> TransformMany<TNewOutput> (Func<TOutput, IEnumerable<TNewOutput>> process)
		{
			return new DataflowTranformManyBuilder<TStart, TOutput, TNewOutput> (this, process);
		}
	}
}