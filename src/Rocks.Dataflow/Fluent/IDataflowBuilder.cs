using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent
{
	public interface IDataflowBuilder<TInput, out TOutput> : IDataflowStartBuilder<TInput>
	{
		/// <summary>
		///     Builds the starting and final blocks of the dataflow.
		/// </summary>
		[NotNull]
		IDataflowBuilderBuildResult<TInput, TOutput> Build ();
	}
}