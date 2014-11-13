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
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public static DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> Join<TStart, TParent, TItem, TOutput> (
			[NotNull] this IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
			Func<SplitJoinResult<TParent, TItem>, Task<TOutput>> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> (previousBuilder, process);
		}


		/// <summary>
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public static DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> Join<TStart, TParent, TItem, TOutput> (
			[NotNull] this IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
			Func<SplitJoinResult<TParent, TItem>, TOutput> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> (previousBuilder, process);
		}


		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public static DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> Join<TStart, TParent, TItem> (
			[NotNull] this IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder)
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> (previousBuilder);
		}
	}
}