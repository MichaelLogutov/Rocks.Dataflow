using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.Builders
{
	public class DataflowTranformManyBuilder<TStart, TInput, TOutput> :
		DataflowBuilder<DataflowTranformManyBuilder<TStart, TInput, TOutput>, TStart, TInput, TOutput>
	{
		#region Private fields

		private readonly Func<TInput, Task<IEnumerable<TOutput>>> processAsync;
		private readonly Func<TInput, IEnumerable<TOutput>> processSync;

		#endregion

		#region Construct

		public DataflowTranformManyBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                                    [NotNull] Func<TInput, Task<IEnumerable<TOutput>>> processAsync)
			: base (previousBuilder)
		{
			if (processAsync == null)
				throw new ArgumentNullException ("processAsync");

			this.processAsync = processAsync;
		}


		public DataflowTranformManyBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                                    [NotNull] Func<TInput, IEnumerable<TOutput>> processSync)
			: base (previousBuilder)
		{
			if (processSync == null)
				throw new ArgumentNullException ("processSync");

			this.processSync = processSync;
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TBuilder}" /> methods.
		/// </summary>
		protected override DataflowTranformManyBuilder<TStart, TInput, TOutput> Builder { get { return this; } }

		#endregion

		#region Protected methods

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<TInput, TOutput> CreateBlock ()
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
							var result = await this.processAsync (input).ConfigureAwait (false);

							return result;
						}
						catch (Exception ex)
						{
							var logger = input as IDataflowErrorLogger;
							if (logger != null)
								logger.OnException (ex);

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
							var result = this.processSync (input);

							return result;
						}
						catch (Exception ex)
						{
							var logger = input as IDataflowErrorLogger;
							if (logger != null)
								logger.OnException (ex);

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