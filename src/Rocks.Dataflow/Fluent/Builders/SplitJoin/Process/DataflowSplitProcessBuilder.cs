using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Process
{
	public partial class DataflowSplitProcessBuilder<TStart, TParent, TItem> :
		DataflowBuilder<DataflowSplitProcessBuilder<TStart, TParent, TItem>, TStart, SplitJoinItem<TParent, TItem>, SplitJoinItem<TParent, TItem>>
	{
		private readonly Func<TParent, TItem, Task> processAsync;
		private readonly Action<TParent, TItem> processSync;


		public DataflowSplitProcessBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                    [NotNull] Func<TParent, TItem, Task> processAsync)
			: base (previousBuilder)
		{
			if (processAsync == null)
				throw new ArgumentNullException (nameof(processAsync));

			this.processAsync = processAsync;
		}


		public DataflowSplitProcessBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                    [NotNull] Action<TParent, TItem> processSync)
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
		protected override DataflowSplitProcessBuilder<TStart, TParent, TItem> Builder => this;


		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<SplitJoinItem<TParent, TItem>, SplitJoinItem<TParent, TItem>> CreateBlock ()
		{
			var block = this.processAsync != null
				            ? DataflowSplitJoin.CreateProcessBlockAsync (this.processAsync, this.options, this.DefaultExceptionLogger)
				            : DataflowSplitJoin.CreateProcessBlock (this.processSync, this.options, this.DefaultExceptionLogger);

			return block;
		}
	}
}