using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Rocks.Dataflow.SplitJoin
{
	/// <summary>
	///     An item with the data for the split-join dataflow pipeline processing.
	/// </summary>
	[DebuggerDisplay ("Parent = {Parent}, Item = {Item}, Result = {Result}")]
	public class SplitJoinItem<TParent, TItem>
	{
		private readonly TParent parent;
		private readonly TItem item;
		private readonly int totalItemsCount;

		private SplitJoinItemResult? result;


		public SplitJoinItem ([NotNull] TParent parent, TItem item, int totalItemsCount)
		{
			if (totalItemsCount <= 0)
			{
				throw new ArgumentOutOfRangeException (nameof(totalItemsCount),
				                                       totalItemsCount,
				                                       "totalItemsCount can not be less or equal than zero " +
				                                       "and it's having value of " + totalItemsCount);
			}

			this.parent = parent;
			this.item = item;
			this.totalItemsCount = totalItemsCount;
		}


		/// <summary>
		///     Parent data of the current <see cref="Item" />.
		/// </summary>
		public TParent Parent => this.parent;

		/// <summary>
		///     Item data.
		/// </summary>
		public TItem Item => this.item;

		/// <summary>
		///     Executed result. Default is null.
		/// </summary>
		public SplitJoinItemResult? Result => this.result;

		/// <summary>
		///     The total number of items that was generated (splitted) from <see cref="Parent" />.
		/// </summary>
		public int TotalItemsCount => this.totalItemsCount;

		/// <summary>
		///     The latest exception that was passed to <see cref="Failed" /> method.
		/// </summary>
		public Exception Exception { get; private set; }


		/// <summary>
		///     Signals that processing of the <see cref="Item" /> has been started.
		/// </summary>
		public void StartProcessing ()
		{
			this.result = null;
		}


		/// <summary>
		///     Signals that <see cref="Item" /> has been successfully processed within dataflow block.
		/// </summary>
		public void CompletedSuccessfully ()
		{
			this.result = SplitJoinItemResult.Success;
		}


		/// <summary>
		///     Signals that there was an error while processing the <see cref="Item" /> within dataflow block.
		/// </summary>
		public void Failed ([NotNull] Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException (nameof(exception));

			this.result = SplitJoinItemResult.Failure;
			this.Exception = exception;
		}


		public override string ToString ()
		{
			return string.Format ("{1} of {0} ({2})", this.Parent, this.Item, this.Result);
		}
	}
}