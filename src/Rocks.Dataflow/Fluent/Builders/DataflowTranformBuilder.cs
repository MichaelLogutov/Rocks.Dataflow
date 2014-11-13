using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.Builders
{
	public class DataflowTranformBuilder<TStart, TInput, TOutput> :
		DataflowBuilder<DataflowTranformBuilder<TStart, TInput, TOutput>, TStart, TInput, TOutput>
	{
		#region Private fields

		private readonly Func<TInput, Task<TOutput>> processAsync;
		private readonly Func<TInput, TOutput> processSync;

		#endregion

		#region Construct

		public DataflowTranformBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                                [NotNull] Func<TInput, Task<TOutput>> processAsync)
			: base (previousBuilder)
		{
			if (processAsync == null)
				throw new ArgumentNullException ("processAsync");

			this.processAsync = processAsync;
		}


		public DataflowTranformBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                                [NotNull] Func<TInput, TOutput> processSync)
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
		protected override DataflowTranformBuilder<TStart, TInput, TOutput> Builder { get { return this; } }

		#endregion

		#region Protected methods

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<TInput, TOutput> CreateBlock ()
		{
			TransformBlock<TInput, TOutput> block;

			if (this.processAsync != null)
			{
				block = new TransformBlock<TInput, TOutput>
					(async input =>
					{
						// ReSharper disable once CompareNonConstrainedGenericWithNull
						if (input == null)
							return default (TOutput);

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

							return default (TOutput);
						}
					},
					 this.options);
			}
			else
			{
				block = new TransformBlock<TInput, TOutput>
					(input =>
					{
						// ReSharper disable once CompareNonConstrainedGenericWithNull
						if (input == null)
							return default (TOutput);

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

							return default (TOutput);
						}
					},
					 this.options);
			}

			return block;
		}

		#endregion
	}
}