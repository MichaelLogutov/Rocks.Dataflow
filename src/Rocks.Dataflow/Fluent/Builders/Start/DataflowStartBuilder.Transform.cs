using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Tranform;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
	public partial class DataflowStartBuilder<TStart>
	{
		/// <summary>
		///     Continues the dataflow from <see cref="TransformBlock{TStart,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowTranformBuilder<TStart, TStart, TOutput> TransformAsync<TOutput> (Func<TStart, Task<TOutput>> process)
		{
			return new DataflowTranformBuilder<TStart, TStart, TOutput> (null, process);
		}


		/// <summary>
		///     Continues the dataflow from <see cref="TransformBlock{TStart,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowTranformBuilder<TStart, TStart, TOutput> Transform<TOutput> (Func<TStart, TOutput> process)
		{
			return new DataflowTranformBuilder<TStart, TStart, TOutput> (null, process);
		}
	}
}