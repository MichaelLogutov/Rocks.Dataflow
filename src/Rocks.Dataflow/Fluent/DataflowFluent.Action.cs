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
		///     Creates the dataflow from the single <see cref="ActionBlock{TInput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TInput, TInput> Action<TInput> (Func<TInput, Task> process)
		{
			return new DataflowActionBuilder<TInput, TInput> (null, process);
		}


		/// <summary>
		///     Creates the dataflow from the single <see cref="ActionBlock{TInput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TInput, TInput> Action<TInput> (Action<TInput> process)
		{
			return new DataflowActionBuilder<TInput, TInput> (null, process);
		}


		/// <summary>
		///     Ends the dataflow with the <see cref="ActionBlock{TInput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TStart, TInput> Action<TStart, TInput> ([NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
		                                                                            Func<TInput, Task> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowActionBuilder<TStart, TInput> (previousBuilder, process);
		}


		/// <summary>
		///     Ends the dataflow with the <see cref="ActionBlock{TInput}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TStart, TInput> Action<TStart, TInput> ([NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
		                                                                            Action<TInput> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowActionBuilder<TStart, TInput> (previousBuilder, process);
		}
	}
}