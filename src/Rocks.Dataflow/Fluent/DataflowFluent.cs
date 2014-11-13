using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent
{
	public static partial class DataflowFluent
	{
		/// <summary>
		///     Creates <see cref="Dataflow{TInput}" /> from the currently configured blocks.
		/// </summary>
		[NotNull]
		public static Dataflow<TStart> CreateDataflow<TStart, TOutput> (this IDataflowBuilder<TStart, TOutput> builder)
		{
			var build_result = builder.Build ();

			return new Dataflow<TStart> (build_result.StartingBlock, build_result.FinalBlock);
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