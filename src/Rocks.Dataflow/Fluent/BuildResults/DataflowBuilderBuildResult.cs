using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.BuildResults
{
	public class DataflowBuilderBuildResult<TInput, TOutput> : IDataflowBuilderBuildResult<TInput, TOutput>,
	                                                           IDataflowFinalBuilderBuildResult<TInput>
	{
		#region Private fields

		private readonly ITargetBlock<TInput> startingBlock;
		private readonly ISourceBlock<TOutput> finalBlock;

		#endregion

		#region Construct

		public DataflowBuilderBuildResult ([NotNull] ITargetBlock<TInput> startingBlock, [NotNull] ISourceBlock<TOutput> finalBlock)
		{
			if (startingBlock == null)
				throw new ArgumentNullException ("startingBlock");

			if (finalBlock == null)
				throw new ArgumentNullException ("finalBlock");

			this.startingBlock = startingBlock;
			this.finalBlock = finalBlock;
		}

		#endregion

		#region IDataflowBuilderBuildResult<TInput,TOutput> Members

		/// <summary>
		///     The starting block of the dataflow.
		/// </summary>
		public ITargetBlock<TInput> StartingBlock { get { return this.startingBlock; } }

		/// <summary>
		///     The final block of the dataflow.
		/// </summary>
		public ISourceBlock<TOutput> FinalBlock { get { return this.finalBlock; } }

		#endregion

		#region IDataflowFinalBuilderBuildResult<TInput> Members

		/// <summary>
		///     The final block of the dataflow.
		/// </summary>
		IDataflowBlock IDataflowFinalBuilderBuildResult<TInput>.FinalBlock { get { return this.FinalBlock; } }

		#endregion
	}
}