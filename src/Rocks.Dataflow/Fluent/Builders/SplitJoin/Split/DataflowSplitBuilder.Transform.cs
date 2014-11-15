using System;
using System.Threading.Tasks;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Process;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Transform;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Split
{
	public partial class DataflowSplitBuilder<TStart, TInput, TItem>
	{
		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessBuilder<TStart, TInput, TItem> SplitProcessAsync (Func<TInput, TItem, Task> process)
		{
			return new DataflowSplitProcessBuilder<TStart, TInput, TItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessBuilder<TStart, TInput, TItem> SplitProcess (Action<TInput, TItem> process)
		{
			return new DataflowSplitProcessBuilder<TStart, TInput, TItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessTransformBuilder<TStart, TInput, TItem, TOutputItem> SplitTransformAsync<TOutputItem> (
			Func<TInput, TItem, Task<TOutputItem>> process)
		{
			return new DataflowSplitProcessTransformBuilder<TStart, TInput, TItem, TOutputItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessTransformBuilder<TStart, TInput, TItem, TOutputItem> SplitTransform<TOutputItem> (
			Func<TInput, TItem, TOutputItem> process)
		{
			return new DataflowSplitProcessTransformBuilder<TStart, TInput, TItem, TOutputItem> (this, process);
		}
	}
}