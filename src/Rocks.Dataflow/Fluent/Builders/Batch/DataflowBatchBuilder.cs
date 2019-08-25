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
            ISourceBlock<TInput[]> ending_block;

            var batch_block = (BatchBlock<TInput>) this.CreateBlock();

            if (this.batchWaitingTimeout == null)
            {
                starting_block = batch_block as ITargetBlock<TStart>;
                ending_block = batch_block;
            }
            else
            {
                Timer timer = null;
                var timeout = (long) this.batchWaitingTimeout.Value.TotalMilliseconds;

                timer = new Timer(
                    callback: _ =>
                              {
                                  batch_block.TriggerBatch();
                                  // ReSharper disable once AccessToModifiedClosure
                                  timer?.Change(timeout, -1);
                              },
                    state: null,
                    dueTime: timeout * 2,
                    period: -1);

                var after_batch_block = new TransformBlock<TInput[], TInput[]>(
                    input =>
                    {
                        timer.Change(timeout, -1);
                        return input;
                    },
                    new ExecutionDataflowBlockOptions
                    {
                        BoundedCapacity = this.options.BoundedCapacity,
                        MaxDegreeOfParallelism = 1,
                        EnsureOrdered = this.options.EnsureOrdered
                    });

                batch_block.LinkWithCompletionPropagation(after_batch_block);

                starting_block = batch_block as ITargetBlock<TStart>;
                ending_block = after_batch_block;
            }

            if (this.previousBuilder != null)
            {
                var previous_build_result = this.previousBuilder.Build();
                previous_build_result.FinalBlock.LinkWithCompletionPropagation((ITargetBlock<TInput>) starting_block);

                starting_block = previous_build_result.StartingBlock;
            }

            if (starting_block == null)
            {
                throw new InvalidOperationException(string.Format("Block {0} can not be casted to the type of the starting block {1}.",
                                                                  batch_block.GetType(),
                                                                  typeof(ITargetBlock<TStart>)));
            }

            var result = new DataflowBuilderBuildResult<TStart, TInput[]>(starting_block, ending_block);

            return result;
        }


        /// <summary>
        ///     Creates a dataflow block from current configuration.
        /// </summary>
        protected override IPropagatorBlock<TInput, TInput[]> CreateBlock()
        {
            return new BatchBlock<TInput>(this.batchSize,
                                          new GroupingDataflowBlockOptions
                                          {
                                              BoundedCapacity = this.options.BoundedCapacity,
                                              EnsureOrdered = this.options.EnsureOrdered
                                          });
        }
    }
}