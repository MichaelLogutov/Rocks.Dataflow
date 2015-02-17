using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Extensions;
using Rocks.Dataflow.Fluent.Builders.Action;
using Rocks.Dataflow.Fluent.Builders.SplitJoin.Join;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
    public partial class DataflowStartBuilder<TStart>
    {
        /// <summary>
        ///     Ends the dataflow from the <see cref="ActionBlock{TStart}" /> using <paramref name="process"/> as a body.
        /// </summary>
        public DataflowActionBuilder<TStart, TStart> ActionAsync (Func<TStart, Task> process)
        {
            if (this.GetType ().Implements (typeof (DataflowFinalBuilder<,,>)))
                throw new InvalidOperationException ("Dataflow already has ending block and can not add another one.");

            return new DataflowActionBuilder<TStart, TStart> (null, process);
        }


        /// <summary>
        ///     Ends the dataflow from the <see cref="ActionBlock{TStart}" /> using <paramref name="process"/> as a body.
        /// </summary>
        public DataflowActionBuilder<TStart, TStart> Action (Action<TStart> process)
        {
            if (this.GetType ().Implements (typeof (DataflowFinalBuilder<,,>)))
                throw new InvalidOperationException ("Dataflow already has ending block and can not add another one.");

            return new DataflowActionBuilder<TStart, TStart> (null, process);
        }
    }
}