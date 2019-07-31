using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.BuildResults
{
	public class DataflowFinalBuilderBuildResult<TInput> : IDataflowFinalBuilderBuildResult<TInput>
	{
		private readonly ITargetBlock<TInput> startingBlock;
		private readonly IDataflowBlock finalBlock;


		public DataflowFinalBuilderBuildResult ([NotNull] ITargetBlock<TInput> startingBlock, [NotNull] IDataflowBlock finalBlock)
		{
			if (startingBlock == null)
				throw new ArgumentNullException (nameof(startingBlock));

			if (finalBlock == null)
				throw new ArgumentNullException (nameof(finalBlock));

			this.startingBlock = startingBlock;
			this.finalBlock = finalBlock;
		}


		/// <summary>
		///     The starting block of the dataflow.
		/// </summary>
		public ITargetBlock<TInput> StartingBlock => this.startingBlock;

		/// <summary>
		///     The final block of the dataflow.
		/// </summary>
		public IDataflowBlock FinalBlock => this.finalBlock;
	}
}