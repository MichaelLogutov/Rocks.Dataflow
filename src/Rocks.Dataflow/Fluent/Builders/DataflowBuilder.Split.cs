using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Split;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract partial class DataflowBuilder<TBuilder, TStart, TInput, TOutput>
	{
		/// <summary>
		///     Continues the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public DataflowSplitBuilder<TStart, TOutput, TItem> SplitToAsync<TItem> (Func<TOutput, Task<IReadOnlyList<TItem>>> getItems)
		{
			return new DataflowSplitBuilder<TStart, TOutput, TItem> (this, getItems);
		}


		/// <summary>
		///     Continues the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public DataflowSplitBuilder<TStart, TOutput, TItem> SplitTo<TItem> (Func<TOutput, IReadOnlyList<TItem>> getItems)
		{
			return new DataflowSplitBuilder<TStart, TOutput, TItem> (this, getItems);
		}
	}
}