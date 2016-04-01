using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.Builders.Tranform
{
	public class DataflowProcessBuilder<TStart, TInput> :
		DataflowBuilder<DataflowProcessBuilder<TStart, TInput>, TStart, TInput, TInput>
	{
		#region Private fields

		private readonly Func<TInput, Task> processAsync;
		private readonly Action<TInput> processSync;

		#endregion

		#region Construct

		public DataflowProcessBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                               [NotNull] Func<TInput, Task> processAsync)
			: base (previousBuilder)
		{
			if (processAsync == null)
				throw new ArgumentNullException (nameof(processAsync));

			this.processAsync = processAsync;
		}


		public DataflowProcessBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder,
		                               [NotNull] Action<TInput> processSync)
			: base (previousBuilder)
		{
			if (processSync == null)
				throw new ArgumentNullException (nameof(processSync));

			this.processSync = processSync;
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder,TInput}" /> methods.
		/// </summary>
		protected override DataflowProcessBuilder<TStart, TInput> Builder { get { return this; } }

		#endregion

		#region Protected methods

		/// <summary>
		///     Creates a dataflow block from current configuration.
		/// </summary>
		protected override IPropagatorBlock<TInput, TInput> CreateBlock ()
		{
			TransformBlock<TInput, TInput> block;

			if (this.processAsync != null)
			{
				block = new TransformBlock<TInput, TInput>
					(async input =>
					{
						// ReSharper disable once CompareNonConstrainedGenericWithNull
						if (input == null)
							return default (TInput);

						try
						{
							await this.processAsync (input).ConfigureAwait (false);

							return input;
						}
						catch (Exception ex)
						{
							var logger = input as IDataflowErrorLogger;
							if (logger != null)
								logger.OnException (ex);
							else if (this.DefaultExceptionLogger != null)
								this.DefaultExceptionLogger (ex, input);

							return default (TInput);
						}
					},
					 this.options);
			}
			else
			{
				block = new TransformBlock<TInput, TInput>
					(input =>
					 {
						// ReSharper disable once CompareNonConstrainedGenericWithNull
						if (input == null)
							return default (TInput);

						try
						{
							this.processSync (input);

							return input;
						}
						catch (Exception ex)
						{
							var logger = input as IDataflowErrorLogger;
							if (logger != null)
								logger.OnException (ex);
							else if (this.DefaultExceptionLogger != null)
								this.DefaultExceptionLogger (ex, input);

							return default (TInput);
						}
					},
					 this.options);
			}

			return block;
		}

		#endregion
	}
}