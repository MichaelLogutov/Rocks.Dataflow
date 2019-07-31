using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.BuildResults
{
	public class DataflowBuilderBuildResult<TInput, TOutput> : IDataflowBuilderBuildResult<TInput, TOutput>,
	                                                           IDataflowFinalBuilderBuildResult<TInput>
	{
		private readonly ITargetBlock<TInput> startingBlock;
		private readonly ISourceBlock<TOutput> finalBlock;


		public DataflowBuilderBuildResult ([NotNull] ITargetBlock<TInput> startingBlock, [NotNull] ISourceBlock<TOutput> finalBlock)
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
		public ISourceBlock<TOutput> FinalBlock => this.finalBlock;

		/// <summary>
		///     The final block of the dataflow.
		/// </summary>
		IDataflowBlock IDataflowFinalBuilderBuildResult<TInput>.FinalBlock => this.FinalBlock;
	}
}