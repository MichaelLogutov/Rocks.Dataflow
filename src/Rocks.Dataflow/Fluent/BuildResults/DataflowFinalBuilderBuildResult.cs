using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.BuildResults
{
	public class DataflowFinalBuilderBuildResult<TInput> : IDataflowFinalBuilderBuildResult<TInput>
	{
		#region Private fields

		private readonly ITargetBlock<TInput> startingBlock;
		private readonly IDataflowBlock finalBlock;

		#endregion

		#region Construct

		public DataflowFinalBuilderBuildResult ([NotNull] ITargetBlock<TInput> startingBlock, [NotNull] IDataflowBlock finalBlock)
		{
			if (startingBlock == null)
				throw new ArgumentNullException (nameof(startingBlock));

			if (finalBlock == null)
				throw new ArgumentNullException (nameof(finalBlock));

			this.startingBlock = startingBlock;
			this.finalBlock = finalBlock;
		}

		#endregion

		#region IDataflowFinalBuilderBuildResult<TInput> Members

		/// <summary>
		///     The starting block of the dataflow.
		/// </summary>
		public ITargetBlock<TInput> StartingBlock { get { return this.startingBlock; } }

		/// <summary>
		///     The final block of the dataflow.
		/// </summary>
		public IDataflowBlock FinalBlock { get { return this.finalBlock; } }

		#endregion
	}
}