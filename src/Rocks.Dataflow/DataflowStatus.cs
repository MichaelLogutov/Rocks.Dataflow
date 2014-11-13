namespace Rocks.Dataflow
{
	/// <summary>
	///     Represents an enumeration of possible <see cref="Dataflow" /> statuses.
	/// </summary>
	public enum DataflowStatus
	{
		/// <summary>
		///     Dataflow created, but not yet started.
		/// </summary>
		NotStarted,

		/// <summary>
		///     Dataflow is in progress.
		/// </summary>
		InProgress,

		/// <summary>
		///     All data has been sent but dataflow is in progress.
		/// </summary>
		AllDataSent,

		/// <summary>
		///     Dataflow is completed and all data is processed.
		/// </summary>
		Completed
	}
}