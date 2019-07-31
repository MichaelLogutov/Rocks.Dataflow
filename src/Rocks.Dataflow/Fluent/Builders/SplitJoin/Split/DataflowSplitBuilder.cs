using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Split
{
	public partial class DataflowSplitBuilder<TStart, TInput, TItem> :
		DataflowBuilder<DataflowSplitBuilder<TStart, TInput, TItem>, TStart, TInput, SplitJoinItem<TInput, TItem>>
	{
		private readonly Func<TInput, Task<IReadOnlyList<TItem>>> getItemsAsync;
		private readonly Func<TInput, IReadOnlyList<TItem>> getItemsSync;


		public DataflowSplitBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                             [NotNull] Func<TInput, Task<IReadOnlyList<TItem>>> getItemsAsync)
			: base (previousBuilder)
		{
			if (getItemsAsync == null)
				throw new ArgumentNullException (nameof(getItemsAsync));

			this.getItemsAsync = getItemsAsync;
		}


		public DataflowSplitBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                             [NotNull] Func<TInput, IReadOnlyList<TItem>> getItemsSync)
			: base (previousBuilder)
		{
			if (getItemsSync == null)
				throw new ArgumentNullException (nameof(getItemsSync));

			this.getItemsSync = getItemsSync;
		}


		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder,TInput}" /> methods.
		/// </summary>
		protected override DataflowSplitBuilder<TStart, TInput, TItem> Builder => this;


		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<TInput, SplitJoinItem<TInput, TItem>> CreateBlock ()
		{
			var block = this.getItemsAsync != null
				            ? DataflowSplitJoin.CreateSplitBlockAsync (this.getItemsAsync, this.options, this.DefaultExceptionLogger)
				            : DataflowSplitJoin.CreateSplitBlock (this.getItemsSync, this.options, this.DefaultExceptionLogger);

			return block;
		}
	}
}