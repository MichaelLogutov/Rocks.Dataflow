using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent
{
	public interface IDataflowFinalBuilder<TInput> : IDataflowStartBuilder<TInput>
	{
		/// <summary>
		///     Builds the starting and final blocks of the dataflow.
		/// </summary>
		[NotNull]
		IDataflowFinalBuilderBuildResult<TInput> Build ();
	}
}