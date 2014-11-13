using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent
{
	public interface IDataflowFinalBuilder<in TInput>
	{
		/// <summary>
		///     Builds the starting and final blocks of the dataflow.
		/// </summary>
		[NotNull]
		IDataflowFinalBuilderBuildResult<TInput> Build ();
	}
}