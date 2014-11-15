using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Transform
{
	public partial class DataflowSplitProcessTransformBuilder<TStart, TParent, TInputItem, TOutputItem> :
		DataflowBuilder<
			DataflowSplitProcessTransformBuilder<TStart, TParent, TInputItem, TOutputItem>,
			TStart,
			SplitJoinItem<TParent, TInputItem>, SplitJoinItem<TParent, TOutputItem>>
	{
		#region Private fields

		private readonly Func<TParent, TInputItem, Task<TOutputItem>> processAsync;
		private readonly Func<TParent, TInputItem, TOutputItem> processSync;

		#endregion

		#region Construct

		public DataflowSplitProcessTransformBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TInputItem>> previousBuilder,
		                                             [NotNull] Func<TParent, TInputItem, Task<TOutputItem>> processAsync)
			: base (previousBuilder)
		{
			if (processAsync == null)
				throw new ArgumentNullException ("processAsync");

			this.processAsync = processAsync;
		}


		public DataflowSplitProcessTransformBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TInputItem>> previousBuilder,
		                                             [NotNull] Func<TParent, TInputItem, TOutputItem> processSync)
			: base (previousBuilder)
		{
			if (processSync == null)
				throw new ArgumentNullException ("processSync");

			this.processSync = processSync;
		}

		#endregion

		#region IDataflowSplitProcessBuilder<TStart,TParent,TOutputItem> Members

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<SplitJoinItem<TParent, TInputItem>, SplitJoinItem<TParent, TOutputItem>> CreateBlock ()
		{
			var block = this.processAsync != null
				            ? DataflowSplitJoin.CreateProcessBlock (this.processAsync, this.options)
				            : DataflowSplitJoin.CreateProcessBlock (this.processSync, this.options);

			return block;
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder}" /> methods.
		/// </summary>
		protected override DataflowSplitProcessTransformBuilder<TStart, TParent, TInputItem, TOutputItem> Builder { get { return this; } }

		#endregion
	}
}