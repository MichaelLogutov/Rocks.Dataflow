using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.Extensions;
using Rocks.Dataflow.Fluent.BuildResults;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract partial class DataflowBuilder<TBuilder, TStart, TInput, TOutput> : DataflowExecutionBlockBuilder<TStart, TBuilder, TInput>,
	                                                                                   IDataflowBuilder<TStart, TOutput>
	{
		protected DataflowBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder)
			: base (previousBuilder)
		{
		}


		/// <summary>
		///     Builds the starting and final blocks of the dataflow.
		/// </summary>
		IDataflowBuilderBuildResult<TStart, TOutput> IDataflowBuilder<TStart, TOutput>.Build ()
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

			var result = new DataflowBuilderBuildResult<TStart, TOutput> (starting_block, current_block);

			return result;
		}


		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected abstract IPropagatorBlock<TInput, TOutput> CreateBlock ();
	}
}