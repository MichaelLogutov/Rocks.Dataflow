using System;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Batch;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
    public partial class DataflowStartBuilder<TStart>
    {
        /// <summary>
        ///     Continues the dataflow with the <see cref="BatchBlock{T}" /> using <paramref name="batchSize"/> as
        ///     size of the batch.<br />
        ///     If <paramref name="batchWaitingTimeout"/> is specified, then after specified time batch processing will be
        ///     triggered even if it has less than <paramref name="batchSize"/> items.
        /// </summary>
        public DataflowBatchBuilder<TStart, TStart> Batch(int batchSize, TimeSpan? batchWaitingTimeout = null)
        {
            return new DataflowBatchBuilder<TStart, TStart>(null, batchSize, batchWaitingTimeout);
        }
    }
}