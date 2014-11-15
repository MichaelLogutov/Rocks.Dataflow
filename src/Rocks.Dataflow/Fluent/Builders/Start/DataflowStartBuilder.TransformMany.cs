using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Tranform;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
	public partial class DataflowStartBuilder<TStart>
	{
		/// <summary>
		///     Continues the dataflow from <see cref="TransformManyBlock{TStart,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowTranformManyBuilder<TStart, TStart, TOutput> TransformManyAsync<TOutput> (Func<TStart, Task<IEnumerable<TOutput>>> process)
		{
			return new DataflowTranformManyBuilder<TStart, TStart, TOutput> (null, process);
		}


		/// <summary>
		///     Continues the dataflow from <see cref="TransformManyBlock{TStart,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowTranformManyBuilder<TStart, TStart, TOutput> TransformMany<TOutput> (Func<TStart, IEnumerable<TOutput>> process)
		{
			return new DataflowTranformManyBuilder<TStart, TStart, TOutput> (null, process);
		}
	}
}