using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Join;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Transform
{
	public partial class DataflowSplitProcessTransformBuilder<TStart, TParent, TInputItem, TOutputItem>
	{
		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public DataflowSplitJoinFinalBuilder<TStart, TParent, TOutputItem> SplitJoin ()
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TParent, TOutputItem> (this);
		}


		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public DataflowSplitJoinFinalBuilder<TStart, TParent, TOutputItem> SplitJoinAsync (
			[NotNull] Func<SplitJoinResult<TParent, TOutputItem>, Task> process)
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TParent, TOutputItem> (this, process);
		}


		/// <summary>
		///     Ends the dataflow with the block that joins the splitted items.
		/// </summary>
		public DataflowSplitJoinFinalBuilder<TStart, TParent, TOutputItem> SplitJoin ([NotNull] Action<SplitJoinResult<TParent, TOutputItem>> process)
		{
			return new DataflowSplitJoinFinalBuilder<TStart, TParent, TOutputItem> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public DataflowSplitJoinBuilder<TStart, TParent, TOutputItem, TOutput> SplitJoinIntoAsync<TOutput> (
			Func<SplitJoinResult<TParent, TOutputItem>, Task<TOutput>> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TParent, TOutputItem, TOutput> (this, process);
		}


		/// <summary>
		///     Continues the dataflow with the block that joins the splitted items.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		public DataflowSplitJoinBuilder<TStart, TParent, TOutputItem, TOutput> SplitJoinInto<TOutput> (
			Func<SplitJoinResult<TParent, TOutputItem>, TOutput> process)
		{
			return new DataflowSplitJoinBuilder<TStart, TParent, TOutputItem, TOutput> (this, process);
		}
	}
}