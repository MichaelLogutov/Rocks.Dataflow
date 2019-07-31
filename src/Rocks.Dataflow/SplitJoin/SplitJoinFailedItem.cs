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
		private readonly TItem item;
		private readonly Exception exception;


		public SplitJoinFailedItem (TItem item, [NotNull] Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException (nameof(exception));

			this.item = item;
			this.exception = exception;
		}


		/// <summary>
		///     Item data.
		/// </summary>
		public TItem Item => this.item;


		/// <summary>
		///     Exception that caused the item to fail.
		/// </summary>
		[NotNull]
		public Exception Exception => this.exception;


		public override string ToString ()
		{
			return string.Format ("{1}: {0}", this.Item, this.Exception.Message);
		}
	}
}