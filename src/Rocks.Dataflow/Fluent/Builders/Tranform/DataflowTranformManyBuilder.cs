using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.Builders.Tranform
{
    public class DataflowTranformManyBuilder<TStart, TInput, TOutput> :
        DataflowBuilder<DataflowTranformManyBuilder<TStart, TInput, TOutput>, TStart, TInput, TOutput>
    {
        #region Private fields

        private readonly Func<TInput, IEnumerable<TOutput>> processSync;
        private readonly Func<TInput, Task<IEnumerable<TOutput>>> processAsync;

        #endregion

        #region Construct

        public DataflowTranformManyBuilder([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
                                           [NotNull] Func<TInput, Task<IEnumerable<TOutput>>> process)
            : base(previousBuilder)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            this.processAsync = process;
        }


        public DataflowTranformManyBuilder([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
                                           [NotNull] Func<TInput, IEnumerable<TOutput>> process)
            : base(previousBuilder)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            this.processSync = process;
        }

        #endregion

        #region Protected properties

        /// <summary>
        ///     Gets the builder instance that will be returned from the
        ///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder,TInput}" /> methods.
        /// </summary>
        protected override DataflowTranformManyBuilder<TStart, TInput, TOutput> Builder => this;

        #endregion

        #region Protected methods

        /// <summary>
        ///     Creates a dataflow block from current configuration.
        /// </summary>
        protected override IPropagatorBlock<TInput, TOutput> CreateBlock()
        {
            TransformManyBlock<TInput, TOutput> block;

            if (this.processAsync != null)
            {
                block = new TransformManyBlock<TInput, TOutput>
                    (async input =>
                           {
                               // ReSharper disable once CompareNonConstrainedGenericWithNull
                               if (input == null)
                                   return new TOutput[0];

                               try
                               {
                                   var result = await this.processAsync(input).ConfigureAwait(false);

                                   return result;
                               }
                               catch (Exception ex)
                               {
                                   var logger = input as IDataflowErrorLogger;
                                   if (logger != null)
                                       logger.OnException(ex);
                                   else
                                       this.DefaultExceptionLogger?.Invoke(ex, input);

                                   return new TOutput[0];
                               }
                           },
                     this.options);
            }
            else
            {
                block = new TransformManyBlock<TInput, TOutput>
                    (input =>
                     {
                         // ReSharper disable once CompareNonConstrainedGenericWithNull
                         if (input == null)
                             return new TOutput[0];

                         try
                         {
                             var result = this.processSync(input);

                             return result;
                         }
                         catch (Exception ex)
                         {
                             var logger = input as IDataflowErrorLogger;
                             if (logger != null)
                                 logger.OnException(ex);
                             else
                                 this.DefaultExceptionLogger?.Invoke(ex, input);

                             return new TOutput[0];
                         }
                     },
                     this.options);
            }

            return block;
        }

        #endregion
    }
}