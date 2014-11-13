using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Rocks.Dataflow.SplitJoin
{
	/// <summary>
	///     An intermediate result of processing single item splitted from parent.
	/// </summary>
	internal class SplitJoinIntermediateResult<TItem>
	{
		#region Private fields

		private readonly List<TItem> succeffullyCompletedItems;
		private readonly List<SplitJoinFailedItem<TItem>> failedItems;
		private readonly int totalItemsCount;
		private int completedItemsCount;

		#endregion

		#region Construct

		public SplitJoinIntermediateResult (int totalItemsCount)
		{
			if (totalItemsCount <= 0)
			{
				throw new ArgumentOutOfRangeException ("totalItemsCount",
				                                       totalItemsCount,
				                                       "totalItemsCount can not be less or equal than zero " +
				                                       "and it's having value of " + totalItemsCount);
			}

			this.totalItemsCount = totalItemsCount;
			this.succeffullyCompletedItems = new List<TItem> ();
			this.failedItems = new List<SplitJoinFailedItem<TItem>> ();
		}

		#endregion

		#region Public properties

		/// <summary>
		///     The total number of items that was generated (splitted) from parent.
		/// </summary>
		public int TotalItemsCount { get { return this.totalItemsCount; } }

		#endregion

		#region Public methods

		/// <summary>
		///     Notifies that <paramref name="item" /> was processed.
		///		Note that this method is not thread safe.
		/// </summary>
		public bool Completed<TParent> ([NotNull] SplitJoinItem<TParent, TItem> item)
		{
			if (item == null)
				throw new ArgumentNullException ("item");

			if (this.completedItemsCount >= this.totalItemsCount)
				throw new InvalidOperationException ("Completed items count already equal to total items count.");

			if (item.Result == null)
				throw new InvalidOperationException ("Item has not been completed.");

			switch (item.Result)
			{
				case SplitJoinItemResult.Success:
				{
					this.succeffullyCompletedItems.Add (item.Item);
					break;
				}

				case SplitJoinItemResult.Failure:
				{
					this.failedItems.Add (new SplitJoinFailedItem<TItem> (item.Item, item.Exception));
					break;
				}

				default:
					throw new NotSupportedException ("item.Result: " + item.Result);
			}


			this.completedItemsCount++;

			return this.completedItemsCount == this.totalItemsCount;
		}


		/// <summary>
		///     Creates new list of items that were completed successfully.
		/// </summary>
		public IReadOnlyList<TItem> GetSucceffullyCompletedItems ()
		{
			return this.succeffullyCompletedItems.ToArray ();
		}


		/// <summary>
		///     Creates new list of items that were completed successfully.
		/// </summary>
		public IReadOnlyList<SplitJoinFailedItem<TItem>> GetFailedItems ()
		{
			return this.failedItems.ToArray ();
		}

		#endregion
	}
}