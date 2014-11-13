using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent
{
	public interface IDataflowBuilder<in TInput, out TOutput>
	{
		/// <summary>
		///     Builds the starting and final blocks of the dataflow.
		/// </summary>
		[NotNull]
		IDataflowBuilderBuildResult<TInput, TOutput> Build ();
	}
}