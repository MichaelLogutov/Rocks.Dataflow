using System.Threading.Tasks.Dataflow;

namespace Rocks.Dataflow.Fluent
{
	public interface IDataflowFinalBuilderBuildResult<in TStart>
	{
		/// <summary>
		///     The starting block of the dataflow.
		/// </summary>
		ITargetBlock<TStart> StartingBlock { get; }

		/// <summary>
		///     The final block of the dataflow.
		/// </summary>
		IDataflowBlock FinalBlock { get; }
	}
}