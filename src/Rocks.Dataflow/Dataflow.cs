using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.Exceptions;

namespace Rocks.Dataflow
{
    /// <summary>
    ///     A dataflow incapulation.
    /// </summary>
    [DebuggerDisplay ("{Status}")]
    public class Dataflow<TInput>
    {
        #region Private fields

        private readonly ITargetBlock<TInput> startingBlock;
        private readonly IDataflowBlock finalBlock;
        private DataflowStatus status;
        private Stopwatch stopwatch;

        #endregion

        #region Construct

        public Dataflow ([NotNull] ITargetBlock<TInput> startingBlock, [NotNull] IDataflowBlock finalBlock)
        {
            if (startingBlock == null)
                throw new ArgumentNullException ("startingBlock");

            if (finalBlock == null)
                throw new ArgumentNullException ("finalBlock");

            if (finalBlock.GetType ().GetInterfaces ().Any (type => type.IsGenericType &&
                                                                    type.GetGenericTypeDefinition () == typeof (ISourceBlock<>)))
            {
                throw new ArgumentException (string.Format ("Dataflow can not have final block of type {0} " +
                                                            "because it can not be awaited after completition of it's input.",
                                                            finalBlock.GetType ()),
                                             "finalBlock");
            }

            this.startingBlock = startingBlock;
            this.finalBlock = finalBlock;

            this.status = DataflowStatus.NotStarted;
        }

        #endregion

        #region Public properties

        /// <summary>
        ///     Current dataflow status.
        /// </summary>
        public DataflowStatus Status
        {
            get { return this.status; }
        }

        /// <summary>
        ///     Elapsed time for processing whole dataflow.
        ///     Throws <see cref="InvalidDataflowStatusException" /> if dataflow has not started yet.
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (this.status == DataflowStatus.NotStarted)
                    throw new InvalidDataflowStatusException (this.status);

                return this.stopwatch.Elapsed;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Performs asynchronous processing of all data <paramref name="items" />.
        /// </summary>
        public async Task ProcessAsync ([NotNull] IEnumerable<TInput> items, CancellationToken cancellationToken = default (CancellationToken))
        {
            this.Start ();

            foreach (var item in items)
                await this.startingBlock.SendAsync (item, cancellationToken).ConfigureAwait (false);

            await this.CompleteAsync ().ConfigureAwait (false);
        }


        /// <summary>
        ///     Changes the dataflow <see cref="Status" /> to <see cref="DataflowStatus.InProgress" />.
        /// </summary>
        public void Start ()
        {
            if (this.status != DataflowStatus.NotStarted)
                throw new InvalidDataflowStatusException (this.status);

            this.stopwatch = new Stopwatch ();
            this.stopwatch.Start ();

            this.status = DataflowStatus.InProgress;
        }


        /// <summary>
        ///     Sends data into the dataflow for processing.
        ///     Returns true if dataflow is accepts and consumes the specified <paramref name="item" />.
        /// </summary>
        public Task<bool> SendAsync (TInput item, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (this.status != DataflowStatus.InProgress)
                throw new InvalidDataflowStatusException (this.status);

            return this.startingBlock.SendAsync (item, cancellationToken);
        }


        /// <summary>
        ///     Signals the completion of data sending.
        ///     Returns the task that will be completed when remaining data in dataflow is processed.
        /// </summary>
        public async Task CompleteAsync ()
        {
            if (this.status != DataflowStatus.InProgress)
                throw new InvalidDataflowStatusException (this.status);

            this.startingBlock.Complete ();
            this.status = DataflowStatus.AllDataSent;

            var completion = this.startingBlock == this.finalBlock
                                 ? this.finalBlock.Completion
                                 : Task.WhenAll (this.startingBlock.Completion, this.finalBlock.Completion);

            await completion.ConfigureAwait (false);

            this.stopwatch.Stop ();
            this.status = DataflowStatus.Completed;
        }

        #endregion
    }
}