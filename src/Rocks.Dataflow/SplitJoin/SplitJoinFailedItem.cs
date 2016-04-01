using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Rocks.Dataflow.SplitJoin
{
	/// <summary>
	///     An item of the split-join dataflow that was failed on some part of the processing pipeline.
	/// </summary>
	[DebuggerDisplay ("Item = {Item}, Exception = {Exception}")]
	public class SplitJoinFailedItem<TItem>
	{
		#region Private fields

		private readonly TItem item;
		private readonly Exception exception;

		#endregion

		#region Construct

		public SplitJoinFailedItem (TItem item, [NotNull] Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException (nameof(exception));

			this.item = item;
			this.exception = exception;
		}

		#endregion

		#region Public properties

		/// <summary>
		///     Item data.
		/// </summary>
		public TItem Item { get { return this.item; } }


		/// <summary>
		///     Exception that caused the item to fail.
		/// </summary>
		[NotNull]
		public Exception Exception { get { return this.exception; } }

		#endregion

		#region Public methods

		public override string ToString ()
		{
			return string.Format ("{1}: {0}", this.Item, this.Exception.Message);
		}

		#endregion
	}
}