using System;
using System.Threading.Tasks;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Process;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Transform
{
	public partial class DataflowSplitProcessTransformBuilder<TStart, TParent, TInputItem, TOutputItem>
	{
		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessBuilder<TStart, TParent, TOutputItem> SplitProcessAsync (Func<TParent, TOutputItem, Task> process)
		{
			return new DataflowSplitProcessBuilder<TStart, TParent, TOutputItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessBuilder<TStart, TParent, TOutputItem> SplitProcess (Action<TParent, TOutputItem> process)
		{
			return new DataflowSplitProcessBuilder<TStart, TParent, TOutputItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessTransformBuilder<TStart, TParent, TOutputItem, TNewOutputItem> SplitTransformAsync<TNewOutputItem> (
			Func<TParent, TOutputItem, Task<TNewOutputItem>> process)
		{
			return new DataflowSplitProcessTransformBuilder<TStart, TParent, TOutputItem, TNewOutputItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public DataflowSplitProcessTransformBuilder<TStart, TParent, TOutputItem, TNewOutputItem> SplitTransform<TNewOutputItem> (
			Func<TParent, TOutputItem, TNewOutputItem> process)
		{
			return new DataflowSplitProcessTransformBuilder<TStart, TParent, TOutputItem, TNewOutputItem> (this, process);
		}
	}
}