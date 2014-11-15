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
		#region Private fields

		private readonly Func<TInput, Task<IReadOnlyList<TItem>>> getItemsAsync;
		private readonly Func<TInput, IReadOnlyList<TItem>> getItemsSync;

		#endregion

		#region Construct

		public DataflowSplitBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                             [NotNull] Func<TInput, Task<IReadOnlyList<TItem>>> getItemsAsync)
			: base (previousBuilder)
		{
			if (getItemsAsync == null)
				throw new ArgumentNullException ("getItemsAsync");

			this.getItemsAsync = getItemsAsync;
		}


		public DataflowSplitBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                             [NotNull] Func<TInput, IReadOnlyList<TItem>> getItemsSync)
			: base (previousBuilder)
		{
			if (getItemsSync == null)
				throw new ArgumentNullException ("getItemsSync");

			this.getItemsSync = getItemsSync;
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder}" /> methods.
		/// </summary>
		protected override DataflowSplitBuilder<TStart, TInput, TItem> Builder { get { return this; } }

		#endregion

		#region Protected methods

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<TInput, SplitJoinItem<TInput, TItem>> CreateBlock ()
		{
			var block = this.getItemsAsync != null
				            ? DataflowSplitJoin.CreateSplitBlock (this.getItemsAsync, this.options)
				            : DataflowSplitJoin.CreateSplitBlock (this.getItemsSync, this.options);

			return block;
		}

		#endregion
	}
}