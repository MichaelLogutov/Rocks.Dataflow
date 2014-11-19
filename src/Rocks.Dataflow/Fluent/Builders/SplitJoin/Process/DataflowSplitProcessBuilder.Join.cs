using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Join;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Process
{
	public partial class DataflowSplitProcessBuilder<TStart, TParent, TItem>
	{
		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> SplitJoin ()
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> (this);
		}


		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> SplitJoinAsync ([NotNull] Func<SplitJoinResult<TParent, TItem>, Task> process)
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> (this, process);
		}


		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> SplitJoin ([NotNull] Action<SplitJoinResult<TParent, TItem>> process)
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> SplitJoinIntoAsync<TOutput> (
			[NotNull] Func<SplitJoinResult<TParent, TItem>, Task<TOutput>> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> SplitJoinInto<TOutput> (
			[NotNull] Func<SplitJoinResult<TParent, TItem>, TOutput> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> (this, process);
		}
	}
}