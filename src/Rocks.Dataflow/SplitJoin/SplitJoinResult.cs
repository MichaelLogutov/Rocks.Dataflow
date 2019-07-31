using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Rocks.Dataflow.SplitJoin
{
	/// <summary>
	///     The merged result of the processing splitted items.
	/// </summary>
	[DebuggerDisplay ("Parent = {Parent}, Successfully completed {SuccessfullyCompletedItems.Count} of {TotalItemsCount}")]
	public class SplitJoinResult<TParent, TItem>
	{
		private readonly TParent parent;
		private readonly IReadOnlyList<TItem> successfullyCompletedItems;
		private readonly IReadOnlyList<SplitJoinFailedItem<TItem>> failedItems;
		private readonly int totalItemsCount;


		public SplitJoinResult (TParent parent,
		                        [NotNull] IReadOnlyList<TItem> successfullyCompletedItems,
		                        [NotNull] IReadOnlyList<SplitJoinFailedItem<TItem>> failedItems,
		                        int totalItemsCount)
		{
			if (successfullyCompletedItems == null)
				throw new ArgumentNullException (nameof(successfullyCompletedItems));

			if (failedItems == null)
				throw new ArgumentNullException (nameof(failedItems));

			if (totalItemsCount <= 0)
			{
				throw new ArgumentOutOfRangeException (nameof(totalItemsCount),
				                                       totalItemsCount,
				                                       "totalItemsCount can not be less or equal than zero " +
				                                       "and it's having value of " + totalItemsCount);
			}

			this.parent = parent;
			this.successfullyCompletedItems = successfullyCompletedItems;
			this.failedItems = failedItems;
			this.totalItemsCount = totalItemsCount;
		}


		internal SplitJoinResult (TParent parent, SplitJoinIntermediateResult<TItem> intermediateResult)
		{
			this.parent = parent;
			this.successfullyCompletedItems = intermediateResult.GetSucceffullyCompletedItems ();
			this.failedItems = intermediateResult.GetFailedItems ();
			this.totalItemsCount = intermediateResult.TotalItemsCount;
		}


		/// <summary>
		///     The parent of all splitted items.
		/// </summary>
		public TParent Parent => this.parent;


		/// <summary>
		///     A list of all successfully completed items.
		/// </summary>
		[NotNull]
		public IReadOnlyList<TItem> SuccessfullyCompletedItems => this.successfullyCompletedItems;


		/// <summary>
		///     A list of all failed items.
		/// </summary>
		[NotNull]
		public IReadOnlyList<SplitJoinFailedItem<TItem>> FailedItems => this.failedItems;


		/// <summary>
		///     The total number of items that was generated (splitted) from <see cref="Parent" />.
		///     This number equal to sum of successfull and failed items count.
		/// </summary>
		public int TotalItemsCount => this.totalItemsCount;


		public override string ToString ()
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			return this.Parent == null ? string.Empty : this.Parent.ToString ();
		}
	}
}