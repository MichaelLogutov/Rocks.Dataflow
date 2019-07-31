using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Join
{
	public class DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> :
		DataflowBuilder<DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput>, TStart, SplitJoinItem<TParent, TItem>, TOutput>
	{
		private readonly Func<SplitJoinResult<TParent, TItem>, Task<TOutput>> processAsync;
		private readonly Func<SplitJoinResult<TParent, TItem>, TOutput> processSync;


		public DataflowSplitJoinBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                 [NotNull] Func<SplitJoinResult<TParent, TItem>, Task<TOutput>> processAsync)
			: base (previousBuilder)
		{
			if (processAsync == null)
				throw new ArgumentNullException (nameof(processAsync));

			this.processAsync = processAsync;
		}


		public DataflowSplitJoinBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                 [NotNull] Func<SplitJoinResult<TParent, TItem>, TOutput> processSync)
			: base (previousBuilder)
		{
			if (processSync == null)
				throw new ArgumentNullException (nameof(processSync));

			this.processSync = processSync;
		}


		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder,TInput}" /> methods.
		/// </summary>
		protected override DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> Builder => this;


		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<SplitJoinItem<TParent, TItem>, TOutput> CreateBlock ()
		{
			var block = this.processAsync != null
				            ? DataflowSplitJoin.CreateJoinBlockAsync (this.processAsync, this.DefaultExceptionLogger)
				            : DataflowSplitJoin.CreateJoinBlock (this.processSync, this.DefaultExceptionLogger);

			return block;
		}
	}
}