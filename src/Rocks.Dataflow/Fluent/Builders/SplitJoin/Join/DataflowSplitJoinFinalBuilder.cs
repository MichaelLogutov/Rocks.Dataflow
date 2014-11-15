using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Fluent.Builders.SplitJoin.Join
{
	public class DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> :
		DataflowFinalBuilder<DataflowSplitJoinFinalBuilder<TStart, TParent, TItem>, TStart, SplitJoinItem<TParent, TItem>>
	{
		#region Construct

		public DataflowSplitJoinFinalBuilder ([CanBeNull] IDataflowBuilder<TStart, SplitJoinItem<TParent, TItem>> previousBuilder)
			: base (previousBuilder)
		{
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder}" /> methods.
		/// </summary>
		protected override DataflowSplitJoinFinalBuilder<TStart, TParent, TItem> Builder { get { return this; } }

		#endregion

		#region Protected methods

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override ITargetBlock<SplitJoinItem<TParent, TItem>> CreateBlock ()
		{
			var block = DataflowSplitJoin.CreateFinalJoinBlock<TParent, TItem> ();

			return block;
		}

		#endregion
	}
}