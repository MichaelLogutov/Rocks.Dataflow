using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Join
{
	public class DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> :
		DataflowFinalBuilder<DataflowSplitJoinFinalBuilder<TStart, TParent, TItem>, TStart, SplitJoinItem<TParent, TItem>>
	{
		private readonly Func<SplitJoinResult<TParent, TItem>, Task> processAsync;
		private readonly Action<SplitJoinResult<TParent, TItem>> processSync;


		public DataflowSplitJoinFinalBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder)
			: base (previousBuilder)
		{
		}


		public DataflowSplitJoinFinalBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                      [CanBeNull] Func<SplitJoinResult<TParent, TItem>, Task> processAsync)
			: base (previousBuilder)
		{
			this.processAsync = processAsync;
		}


		public DataflowSplitJoinFinalBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                      [CanBeNull] Action<SplitJoinResult<TParent, TItem>> processSync)
			: base (previousBuilder)
		{
			this.processSync = processSync;
		}


		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder,TInput}" /> methods.
		/// </summary>
		protected override DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> Builder => this;


		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override ITargetBlock<SplitJoinItem<TParent, TItem>> CreateBlock ()
		{
			ITargetBlock<SplitJoinItem<TParent, TItem>> block;

			if (this.processAsync != null)
				block = DataflowSplitJoin.CreateFinalJoinBlockAsync (this.processAsync, this.DefaultExceptionLogger);
			else if (this.processSync != null)
				block = DataflowSplitJoin.CreateFinalJoinBlock (this.processSync, this.DefaultExceptionLogger);
			else
				block = DataflowSplitJoin.CreateFinalJoinBlock<TParent, TItem> ();

			return block;
		}
	}
}