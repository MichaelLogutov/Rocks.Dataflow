using System;
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
		///     Starts the dataflow from <see cref="TransformBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformBuilder<TInput, TInput, TOutput> Transform<TInput, TOutput> (Func<TInput, Task<TOutput>> process)
		{
			return new DataflowTranformBuilder<TInput, TInput, TOutput> (null, process);
		}


		/// <summary>
		///     Starts the dataflow from <see cref="TransformBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformBuilder<TInput, TInput, TOutput> Transform<TInput, TOutput> (Func<TInput, TOutput> process)
		{
			return new DataflowTranformBuilder<TInput, TInput, TOutput> (null, process);
		}


		/// <summary>
		///     Continues the dataflow with the <see cref="TransformBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformBuilder<TStart, TInput, TOutput> Transform<TStart, TInput, TOutput> (
			[NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
			Func<TInput, Task<TOutput>> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowTranformBuilder<TStart, TInput, TOutput> (previousBuilder, process);
		}


		/// <summary>
		///     Continues the dataflow with the <see cref="TransformBlock{TInput,TOutput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowTranformBuilder<TStart, TInput, TOutput> Transform<TStart, TInput, TOutput> (
			[NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
			Func<TInput, TOutput> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowTranformBuilder<TStart, TInput, TOutput> (previousBuilder, process);
		}
	}
}