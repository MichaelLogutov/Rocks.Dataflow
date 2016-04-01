using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;

namespace Rocks.Dataflow.Extensions
{
	[UsedImplicitly]
	public static class DataflowExtensions
	{
		/// <summary>
		///     Gets the <see cref="ExecutionDataflowBlockOptions.MaxDegreeOfParallelism"/> from the given <paramref name="value"/>.
		///     If the <paramref name="value"/> is null, then <see cref="DataflowBlockOptions.Unbounded"/> is returned.
		///     Is the <paramref name="value"/> is 0, then <see cref="Environment.ProcessorCount"/> is returned.
		/// </summary>
		public static int GetMaxDegreeOfParallelism (this int? value)
		{
			if (value == null)
				return DataflowBlockOptions.Unbounded;

			return GetMaxDegreeOfParallelism (value.Value);
		}


		/// <summary>
		///     Gets the <see cref="ExecutionDataflowBlockOptions.MaxDegreeOfParallelism"/> from the given <paramref name="value"/>.
		///     Is the <paramref name="value"/> is 0, then <see cref="Environment.ProcessorCount"/> is returned.
		/// </summary>
		public static int GetMaxDegreeOfParallelism (this int value)
		{
			if (value == 0)
				return Environment.ProcessorCount;

			return value;
		}



		/// <summary>
		///     A shortcut method for <see cref="ISourceBlock{TOutput}.LinkTo" /> with
		///     <see cref="DataflowLinkOptions.PropagateCompletion" /> = true.
		/// </summary>
		public static void LinkWithCompletionPropagation<TContext> (this ISourceBlock<TContext> sourceBlock, ITargetBlock<TContext> targetBlock)
		{
			sourceBlock.LinkTo (targetBlock, new DataflowLinkOptions { PropagateCompletion = true });
		}


		/// <summary>
		///     Performs shallow clone of the options.
		/// </summary>
		public static DataflowBlockOptions Clone ([NotNull] this DataflowBlockOptions source)
		{
			if (source == null)
				throw new ArgumentNullException (nameof(source));

			var result = new DataflowBlockOptions
			             {
				             BoundedCapacity = source.BoundedCapacity,
				             CancellationToken = source.CancellationToken,
				             MaxMessagesPerTask = source.MaxMessagesPerTask,
				             NameFormat = source.NameFormat,
				             TaskScheduler = source.TaskScheduler
			             };

			return result;
		}


		/// <summary>
		///     Performs shallow clone of the options.
		/// </summary>
		public static ExecutionDataflowBlockOptions Clone ([NotNull] this ExecutionDataflowBlockOptions source)
		{
			if (source == null)
				throw new ArgumentNullException (nameof(source));

			var result = new ExecutionDataflowBlockOptions
			             {
				             BoundedCapacity = source.BoundedCapacity,
				             CancellationToken = source.CancellationToken,
				             MaxMessagesPerTask = source.MaxMessagesPerTask,
				             NameFormat = source.NameFormat,
				             TaskScheduler = source.TaskScheduler,
				             MaxDegreeOfParallelism = source.MaxDegreeOfParallelism,
				             SingleProducerConstrained = source.SingleProducerConstrained
			             };

			return result;
		}


		/// <summary>
		///     Performs shallow clone of the options.
		/// </summary>
		public static GroupingDataflowBlockOptions Clone ([NotNull] this GroupingDataflowBlockOptions source)
		{
			if (source == null)
				throw new ArgumentNullException (nameof(source));

			var result = new GroupingDataflowBlockOptions
			             {
				             BoundedCapacity = source.BoundedCapacity,
				             CancellationToken = source.CancellationToken,
				             MaxMessagesPerTask = source.MaxMessagesPerTask,
				             NameFormat = source.NameFormat,
				             TaskScheduler = source.TaskScheduler,
				             Greedy = source.Greedy,
				             MaxNumberOfGroups = source.MaxNumberOfGroups
			             };

			return result;
		}
	}
}