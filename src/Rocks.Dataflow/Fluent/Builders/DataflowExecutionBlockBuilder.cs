using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract class DataflowExecutionBlockBuilder<TBuilder>
	{
		#region Private fields

		/// <summary>
		///     Currently configured execution options for dataflow block.
		/// </summary>
		protected ExecutionDataflowBlockOptions options;

		#endregion

		#region Construct

		protected DataflowExecutionBlockBuilder ()
		{
			this.options = new ExecutionDataflowBlockOptions
			               {
				               BoundedCapacity = 1000,
				               MaxDegreeOfParallelism = Environment.ProcessorCount
			               };
		}

		#endregion

		#region Public methods

		/// <summary>
		///     Specifies <paramref name="options" /> that will be used for creation of actual dataflow block.
		/// </summary>
		public TBuilder WithOptions ([NotNull] ExecutionDataflowBlockOptions options)
		{
			if (options == null)
				throw new ArgumentNullException ("options");

			this.options = options;
			return this.Builder;
		}


		/// <summary>
		///     Specifies <see cref="ExecutionDataflowBlockOptions.MaxDegreeOfParallelism" /> that will be used for creation of
		///     actual dataflow block.
		/// </summary>
		public TBuilder WithMaxDegreeOfParallelism (int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded)
		{
			this.options.MaxDegreeOfParallelism = maxDegreeOfParallelism;
			return this.Builder;
		}


		/// <summary>
		///     Specifies <see cref="ExecutionDataflowBlockOptions.BoundedCapacity" /> that will be used for creation of actual
		///     dataflow block.
		/// </summary>
		public TBuilder WithBoundedCapacity (int boundedCapacity)
		{
			this.options.BoundedCapacity = boundedCapacity;
			return this.Builder;
		}


		/// <summary>
		///     Specifies <see cref="ExecutionDataflowBlockOptions.CancellationToken" /> that will be used for creation of actual
		///     dataflow block.
		/// </summary>
		public TBuilder WithCancelationToken (CancellationToken cancellationToken)
		{
			this.options.CancellationToken = cancellationToken;
			return this.Builder;
		}


		/// <summary>
		///     Specifies <see cref="ExecutionDataflowBlockOptions.TaskScheduler" /> that will be used for creation of actual
		///     dataflow block.
		/// </summary>
		public TBuilder WithTaskScheduler (TaskScheduler taskScheduler)
		{
			this.options.TaskScheduler = taskScheduler;
			return this.Builder;
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TBuilder}" /> methods.
		/// </summary>
		protected abstract TBuilder Builder { get; }

		#endregion
	}
}