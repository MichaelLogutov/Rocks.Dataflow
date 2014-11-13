using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Rocks.Dataflow.SplitJoin
{
	/// <summary>
	///     The merged result of the processing splitted items.
	/// </summary>
	[DebuggerDisplay ("Parent = {Parent}, Succeffully completed {SucceffullyCompletedItems.Count} of {TotalItemsCount}")]
	public class SplitJoinResult<TParent, TItem>
	{
		#region Private fields

		private readonly TParent parent;
		private readonly IReadOnlyList<TItem> succeffullyCompletedItems;
		private readonly IReadOnlyList<SplitJoinFailedItem<TItem>> failedItems;
		private readonly int totalItemsCount;

		#endregion

		#region Construct

		public SplitJoinResult (TParent parent,
		                        [NotNull] IReadOnlyList<TItem> succeffullyCompletedItems,
		                        [NotNull] IReadOnlyList<SplitJoinFailedItem<TItem>> failedItems,
		                        int totalItemsCount)
		{
			if (succeffullyCompletedItems == null)
				throw new ArgumentNullException ("succeffullyCompletedItems");

			if (failedItems == null)
				throw new ArgumentNullException ("failedItems");

			if (totalItemsCount <= 0)
			{
				throw new ArgumentOutOfRangeException ("totalItemsCount",
				                                       totalItemsCount,
				                                       "totalItemsCount can not be less or equal than zero " +
				                                       "and it's having value of " + totalItemsCount);
			}

			this.parent = parent;
			this.succeffullyCompletedItems = succeffullyCompletedItems;
			this.failedItems = failedItems;
			this.totalItemsCount = totalItemsCount;
		}


		internal SplitJoinResult (TParent parent, SplitJoinIntermediateResult<TItem> intermediateResult)
		{
			this.parent = parent;
			this.succeffullyCompletedItems = intermediateResult.GetSucceffullyCompletedItems ();
			this.failedItems = intermediateResult.GetFailedItems ();
			this.totalItemsCount = intermediateResult.TotalItemsCount;
		}

		#endregion

		#region Public properties

		/// <summary>
		///     The parent of all splitted items.
		/// </summary>
		public TParent Parent { get { return this.parent; } }


		/// <summary>
		///     A list of all successfully completed items.
		/// </summary>
		[NotNull]
		public IReadOnlyList<TItem> SucceffullyCompletedItems { get { return this.succeffullyCompletedItems; } }


		/// <summary>
		///     A list of all failed items.
		/// </summary>
		[NotNull]
		public IReadOnlyList<SplitJoinFailedItem<TItem>> FailedItems { get { return this.failedItems; } }


		/// <summary>
		///     The total number of items that was generated (splitted) from <see cref="Parent" />.
		///     This number equal to sum of successfull and failed items count.
		/// </summary>
		public int TotalItemsCount { get { return this.totalItemsCount; } }

		#endregion

		#region Public methods

		public override string ToString ()
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			return this.Parent == null ? string.Empty : this.Parent.ToString ();
		}

		#endregion
	}
}