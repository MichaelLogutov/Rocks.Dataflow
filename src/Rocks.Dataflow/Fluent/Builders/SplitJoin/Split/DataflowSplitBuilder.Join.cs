using System;
using System.Threading.Tasks;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Join;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Split
{
	public partial class DataflowSplitBuilder<TStart, TInput, TItem>
	{
		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public DataflowSplitJoinFinalBuilder<TStart, TInput, TItem> SplitJoin ()
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TInput, TItem> (this);
		}


		/// <summary>
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public DataflowSplitJoinBuilder<TStart, TInput, TItem, TOutput> SplitJoinIntoAsync<TOutput> (
			Func<SplitJoinResult<TInput, TItem>, Task<TOutput>> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TInput, TItem, TOutput> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public DataflowSplitJoinBuilder<TStart, TInput, TItem, TOutput> SplitJoinInto<TOutput> (
			Func<SplitJoinResult<TInput, TItem>, TOutput> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TInput, TItem, TOutput> (this, process);
		}
	}
}