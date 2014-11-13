using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.Fluent.Builders;

namespace Rocks.Dataflow.Fluent
{
	public static partial class DataflowFluent
	{
		/// <summary>
		///     Starts the dataflow from <see cref="TransformManyBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformManyBuilder<TInput, TInput, TOutput> TransformMany<TInput, TOutput> (Func<TInput, Task<IEnumerable<TOutput>>> process)
		{
			return new DataflowTranformManyBuilder<TInput, TInput, TOutput> (null, process);
		}


		/// <summary>
		///     Starts the dataflow from <see cref="TransformManyBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformManyBuilder<TInput, TInput, TOutput> TransformMany<TInput, TOutput> (Func<TInput, IEnumerable<TOutput>> process)
		{
			return new DataflowTranformManyBuilder<TInput, TInput, TOutput> (null, process);
		}


		/// <summary>
		///     Continues the dataflow with the <see cref="TransformManyBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformManyBuilder<TStart, TInput, TOutput> TransformMany<TStart, TInput, TOutput> (
			[NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
			Func<TInput, Task<IEnumerable<TOutput>>> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowTranformManyBuilder<TStart, TInput, TOutput> (previousBuilder, process);
		}


		/// <summary>
		///     Continues the dataflow with the <see cref="TransformManyBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformManyBuilder<TStart, TInput, TOutput> TransformMany<TStart, TInput, TOutput> (
			[NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
			Func<TInput, IEnumerable<TOutput>> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowTranformManyBuilder<TStart, TInput, TOutput> (previousBuilder, process);
		}
	}
}