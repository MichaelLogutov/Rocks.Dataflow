using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.Extensions;
using Rocks.Dataflow.Fluent.BuildResults;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract class DataflowFinalBuilder<TBuilder, TStart, TInput> : DataflowExecutionBlockBuilder<TStart, TBuilder, TInput>,
	                                                                       IDataflowFinalBuilder<TStart>
	{
		#region Construct

		protected DataflowFinalBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder)
			: base (previousBuilder)
		{
		}

		#endregion

		#region IDataflowBuilder<TStart,TOutput> Members

		/// <summary>
		///     Builds the starting and final blocks of the dataflow.
		/// </summary>
		IDataflowFinalBuilderBuildResult<TStart> IDataflowFinalBuilder<TStart>.Build ()
		{
			ITargetBlock<TStart> starting_block;

			var current_block = this.CreateBlock ();

			if (this.previousBuilder != null)
			{
				var previous_build_result = this.previousBuilder.Build ();

				starting_block = previous_build_result.StartingBlock;
				previous_build_result.FinalBlock.LinkWithCompletionPropagation (current_block);
			}
			else
			{
				starting_block = current_block as ITargetBlock<TStart>;
				if (starting_block == null)
				{
					throw new InvalidOperationException (string.Format ("Block {0} can not be casted to the type of the starting block {1}.",
					                                                    current_block.GetType (),
					                                                    typeof (ITargetBlock<TStart>)));
				}
			}

			var result = new DataflowFinalBuilderBuildResult<TStart> (starting_block, current_block);

			return result;
		}

		#endregion

		#region Protected methods

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected abstract ITargetBlock<TInput> CreateBlock ();

		#endregion
	}
}