using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rocks.Dataflow.Fluent.Builders;

namespace Rocks.Dataflow.Fluent
{
	public static partial class DataflowFluent
	{
		/// <summary>
		///     Starts the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public static DataflowSplitBuilder<TInput, TInput, TItem> Split<TInput, TItem> (
			Func<TInput, Task<IReadOnlyList<TItem>>> getItems)
		{
			return new DataflowSplitBuilder<TInput, TInput, TItem> (null, getItems);
		}


		/// <summary>
		///     Starts the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public static DataflowSplitBuilder<TInput, TInput, TItem> Split<TInput, TItem> (
			Func<TInput, IReadOnlyList<TItem>> getItems)
		{
			return new DataflowSplitBuilder<TInput, TInput, TItem> (null, getItems);
		}


		/// <summary>
		///     Continues the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public static DataflowSplitBuilder<TStart, TInput, TItem> Split<TStart, TInput, TItem> (
			[NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
			Func<TInput, Task<IReadOnlyList<TItem>>> getItems)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowSplitBuilder<TStart, TInput, TItem> (previousBuilder, getItems);
		}


		/// <summary>
		///     Continues the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public static DataflowSplitBuilder<TStart, TInput, TItem> Split<TStart, TInput, TItem> (
			[NotNull] this IDataflowBuilder<TStart, TInput> previousBuilder,
			Func<TInput, IReadOnlyList<TItem>> getItems)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowSplitBuilder<TStart, TInput, TItem> (previousBuilder, getItems);
		}
	}
}