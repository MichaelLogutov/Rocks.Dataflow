using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.Extensions;
using Rocks.Dataflow.Fluent.BuildResults;

namespace Rocks.Dataflow.Fluent.Builders.Batch
{
    public class DataflowBatchBuilder<TStart, TInput> :
        DataflowBuilder<DataflowBatchBuilder<TStart, TInput>, TStart, TInput, TInput[]>,
        IDataflowBuilder<TStart, TInput[]>
    {
        private readonly int batchSize;
        private readonly TimeSpan? batchWaitingTimeout;


        public DataflowBatchBuilder([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
                                    int batchSize,
                                    TimeSpan? batchWaitingTimeout)
            : base(previousBuilder)
        {
            this.batchSize = batchSize;
            this.batchWaitingTimeout = batchWaitingTimeout;
        }


        /// <summary>
        ///     Gets the builder instance that will be returned from the
        ///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder,TInput}" /> methods.
        /// </summary>
        protected override DataflowBatchBuilder<TStart, TInput> Builder => this;


        /// <summary>
        ///     Builds the starting and final blocks of the dataflow.
        /// </summary>
        IDataflowBuilderBuildResult<TStart, TInput[]> IDataflowBuilder<TStart, TInput[]>.Build()
        {
            ITargetBlock<TStart> starting_block;
            TransformBlock<TInput, TInput> timer_block = null;
            var batch_block = (BatchBlock<TInput>) this.CreateBlock();

            if (this.batchWaitingTimeout != null)
            {
                var timer = new Timer(_ => batch_block.TriggerBatch());

                timer_block = new TransformBlock<TInput, TInput>(
                    input =>
                    {
                        // ReSharper disable once CompareNonConstrainedGenericWithNull
                        if (input == null)
                            return default(TInput);

                        timer.Change(this.batchWaitingTimeout.Value, Timeout.InfiniteTimeSpan);

                        return input;
                    });

                timer_block.LinkWithCompletionPropagation(batch_block);
            }

            if (this.previousBuilder != null)
            {
                var previous_build_result = this.previousBuilder.Build();

                starting_block = previous_build_result.StartingBlock;
                previous_build_result.FinalBlock.LinkWithCompletionPropagation((ITargetBlock<TInput>) timer_block ?? batch_block);
            }
            else
            {
                starting_block = timer_block != null
                                     ? timer_block as ITargetBlock<TStart>
                                     : batch_block as ITargetBlock<TStart>;
                
                if (starting_block == null)
                {
                    throw new InvalidOperationException(string.Format("Block {0} can not be casted to the type of the starting block {1}.",
                                                                      batch_block.GetType(),
                                                                      typeof(ITargetBlock<TStart>)));
                }
            }

            var result = new DataflowBuilderBuildResult<TStart, TInput[]>(starting_block, batch_block);

            return result;
        }


        /// <summary>
        ///     Creates a dataflow block from current configuration.
        /// </summary>
        protected override IPropagatorBlock<TInput, TInput[]> CreateBlock()
        {
            return new BatchBlock<TInput>(this.batchSize);
        }
    }
}