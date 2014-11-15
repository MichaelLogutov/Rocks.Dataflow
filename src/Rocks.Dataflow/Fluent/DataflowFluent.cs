using JetBrains.Annotations;
using Rocks.Dataflow.Fluent.Builders.Start;

namespace Rocks.Dataflow.Fluent
{
	public static class DataflowFluent
	{
		/// <summary>
		///     Starts the dataflow with the input data of <typeparamref name="TStart" />.
		/// </summary>
		[NotNull]
		public static DataflowStartBuilder<TStart> ReceiveDataOfType<TStart> ()
		{
			return new DataflowStartBuilder<TStart> ();
		}


		/// <summary>
		///     Creates <see cref="Dataflow{TInput}" /> from the currently configured blocks.
		/// </summary>
		[NotNull]
		public static Dataflow<TStart> CreateDataflow<TStart> (this IDataflowFinalBuilder<TStart> builder)
		{
			var build_result = builder.Build ();

			return new Dataflow<TStart> (build_result.StartingBlock, build_result.FinalBlock);
		}
	}
}