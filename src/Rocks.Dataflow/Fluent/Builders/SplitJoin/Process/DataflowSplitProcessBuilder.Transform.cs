using System;
using System.Threading.Tasks;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Transform;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Process
{
	public partial class DataflowSplitProcessBuilder<TStart, TParent, TItem>
	{
		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessBuilder<TStart, TParent, TItem> SplitProcessAsync (Func<TParent, TItem, Task> process)
		{
			return new DataflowSplitProcessBuilder<TStart, TParent, TItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessBuilder<TStart, TParent, TItem> SplitProcess (Action<TParent, TItem> process)
		{
			return new DataflowSplitProcessBuilder<TStart, TParent, TItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessTransformBuilder<TStart, TParent, TItem, TOutputItem> SplitTransformAsync<TOutputItem> (
			Func<TParent, TItem, Task<TOutputItem>> process)
		{
			return new DataflowSplitProcessTransformBuilder<TStart, TParent, TItem, TOutputItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessTransformBuilder<TStart, TParent, TItem, TOutputItem> SplitTransform<TOutputItem> (
			Func<TParent, TItem, TOutputItem> process)
		{
			return new DataflowSplitProcessTransformBuilder<TStart, TParent, TItem, TOutputItem> (this, process);
		}
	}
}