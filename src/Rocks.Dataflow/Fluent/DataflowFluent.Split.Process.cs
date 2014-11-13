using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rocks.Dataflow.Fluent.Builders;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent
{
	public static partial class DataflowFluent
	{
		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public static DataflowSplitProcessBuilder<TStart, TInput, TItem>
			SplitProcess<TStart, TInput, TItem> (
			[NotNull] this IDataflowBuilder<TStart, SplitJoinItem<TInput, TItem>> previousBuilder,
			Func<TInput, TItem, Task> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowSplitProcessBuilder<TStart, TInput, TItem> (previousBuilder, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public static DataflowSplitProcessBuilder<TStart, TInput, TItem>
			SplitProcess<TStart, TInput, TItem> (
			[NotNull] this IDataflowBuilder<TStart, SplitJoinItem<TInput, TItem>> previousBuilder,
			Action<TInput, TItem> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowSplitProcessBuilder<TStart, TInput, TItem> (previousBuilder, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public static DataflowSplitProcessTransformBuilder<TStart, TInput, TInputItem, TOutputItem>
			SplitTransform<TStart, TInput, TInputItem, TOutputItem> (
			[NotNull] this IDataflowBuilder<TStart, SplitJoinItem<TInput, TInputItem>> previousBuilder,
			Func<TInput, TInputItem, Task<TOutputItem>> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowSplitProcessTransformBuilder<TStart, TInput, TInputItem, TOutputItem> (previousBuilder, process);
		}


		/// <summary>
		///     Continues the dataflow with the processing of splitted item using <paramref name="process"/> function.
		/// </summary>
		public static DataflowSplitProcessTransformBuilder<TStart, TInput, TInputItem, TOutputItem>
			SplitTransform<TStart, TInput, TInputItem, TOutputItem> (
			[NotNull] this IDataflowBuilder<TStart, SplitJoinItem<TInput, TInputItem>> previousBuilder,
			Func<TInput, TInputItem, TOutputItem> process)
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowSplitProcessTransformBuilder<TStart, TInput, TInputItem, TOutputItem> (previousBuilder, process);
		}
	}
}