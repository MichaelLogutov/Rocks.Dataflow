using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Split;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
	public partial class DataflowStartBuilder<TStart>
	{
		/// <summary>
		///     Continues the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public DataflowSplitBuilder<TStart, TStart, TItem> SplitToAsync<TItem> (Func<TStart, Task<IReadOnlyList<TItem>>> getItems)
		{
			return new DataflowSplitBuilder<TStart, TStart, TItem> (null, getItems);
		}


		/// <summary>
		///     Continues the dataflow with the block that splits it's processing to multiple items using <paramref name="getItems"/> function.
		/// </summary>
		public DataflowSplitBuilder<TStart, TStart, TItem> SplitTo<TItem> (Func<TStart, IReadOnlyList<TItem>> getItems)
		{
			return new DataflowSplitBuilder<TStart, TStart, TItem> (null, getItems);
		}
	}
}