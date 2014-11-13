namespace Rocks.Dataflow.SplitJoin
{
	public enum SplitJoinItemResult
	{
		/// <summary>
		///     Splitted item was proceeded successfully.
		/// </summary>
		Success,

		/// <summary>
		///     An error occuired while processing splitted item.
		/// </summary>
		Failure
	}
}