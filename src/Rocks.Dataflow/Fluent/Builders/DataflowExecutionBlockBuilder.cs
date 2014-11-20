using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.Fluent.Builders.Start;

namespace Rocks.Dataflow.Fluent.Builders
{
	public abstract class DataflowExecutionBlockBuilder<TStart, TBuilder, TInput> : DataflowStartBuilder<TStart>
	{
		#region Private fields

		/// <summary>
		///     Previous builder in fluent buidling chain.
		///     If null, then current builder is the first one.
		/// </summary>
		[CanBeNull]
		protected readonly IDataflowBuilder<TStart, TInput> previousBuilder;


		/// <summary>
		///     Currently configured execution options for dataflow block.
		/// </summary>
		[NotNull]
		protected ExecutionDataflowBlockOptions options;


		/// <summary>
		///     Default exception logger that will be used if currently
		///     processed item in dataflow block does not implements <see cref="IDataflowErrorLogger" />.
		///     If null - no logging will be performed and exception will be swallowed.
		/// </summary>
		private Action<Exception, object> defaultExceptionLogger;

		#endregion

		#region Construct

		protected DataflowExecutionBlockBuilder ([CanBeNull] IDataflowBuilder<TStart, TInput> previousBuilder)
		{
			this.previousBuilder = previousBuilder;

			var previous_start_builder = this.previousBuilder as DataflowStartBuilder<TStart>;
			if (previous_start_builder != null)
				this.defaultExceptionLogger = previous_start_builder.DefaultExceptionLogger;

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
		///		If this method was not called then by default processor count value is used.
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


		/// <summary>
		///     Sets the default exception logger that will be used if currently
		///     processed item in dataflow block does not implements <see cref="IDataflowErrorLogger" />.
		///     If null - no logging will be performed and exception will be swallowed.
		/// </summary>
		public TBuilder WithDefaultExceptionLogger (Action<Exception, object> logger)
		{
			this.DefaultExceptionLogger = logger;
			return this.Builder;
		}

		#endregion

		#region Protected properties

		/// <summary>
		///     Default exception logger that will be used if currently
		///     processed item in dataflow block does not implements <see cref="IDataflowErrorLogger" />.
		///     If null - no logging will be performed and exception will be swallowed.
		/// </summary>
		protected internal override Action<Exception, object> DefaultExceptionLogger
		{
			get { return this.defaultExceptionLogger; }
			set
			{
				this.defaultExceptionLogger = value;

				if (this.previousBuilder == null)
					return;

				var previous_start_builder = this.previousBuilder as DataflowStartBuilder<TStart>;
				if (previous_start_builder != null)
					previous_start_builder.DefaultExceptionLogger = value;
			}
		}

		/// <summary>
		///     Gets the builder instance that will be returned from the
		///     <see cref="DataflowExecutionBlockBuilder{TStart,TBuilder, TInput}" /> methods.
		/// </summary>
		protected abstract TBuilder Builder { get; }

		#endregion
	}
}