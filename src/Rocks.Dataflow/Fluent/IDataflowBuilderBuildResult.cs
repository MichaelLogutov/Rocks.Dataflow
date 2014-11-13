using System.Threading.Tasks.Dataflow;

namespace Rocks.Dataflow.Fluent
{
	public interface IDataflowBuilderBuildResult<in TInput, out TOutput>
	{
		/// <summary>
		///     The starting block of the dataflow.
		/// </summary>
		ITargetBlock<TInput> StartingBlock { get; }

		/// <summary>
		///     The final block of the dataflow.
		/// </summary>
		ISourceBlock<TOutput> FinalBlock { get; }
	}
}