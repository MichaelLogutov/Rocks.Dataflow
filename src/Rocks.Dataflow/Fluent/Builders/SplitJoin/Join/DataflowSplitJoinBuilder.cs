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
		#region Private fields

		private readonly Func<SplitJoinResult<TParent, TItem>, Task<TOutput>> processAsync;
		private readonly Func<SplitJoinResult<TParent, TItem>, TOutput> processSync;

		#endregion

		#region Construct

		public DataflowSplitJoinBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                 [NotNull] Func<SplitJoinResult<TParent, TItem>, Task<TOutput>> processAsync)
			: base (previousBuilder)
		{
			if (processAsync == null)
				throw new ArgumentNullException ("processAsync");

			this.processAsync = processAsync;
		}


		public DataflowSplitJoinBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder,
		                                 [NotNull] Func<SplitJoinResult<TParent, TItem>, TOutput> processSync)
			: base (previousBuilder)
		{
			if (processSync == null)
				throw new ArgumentNullException ("processSync");

			this.processSync = processSync;
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder,TInput}" /> methods.
		/// </summary>
		protected override DataflowSplitJoinBuilder<TStart, TParent, TItem, TOutput> Builder { get { return this; } }

		#endregion

		#region Protected methods

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<SplitJoinItem<TParent, TItem>, TOutput> CreateBlock ()
		{
			var block = this.processAsync != null
				            ? DataflowSplitJoin.CreateJoinBlock (this.processAsync)
				            : DataflowSplitJoin.CreateJoinBlock (this.processSync);

			return block;
		}

		#endregion
	}
}