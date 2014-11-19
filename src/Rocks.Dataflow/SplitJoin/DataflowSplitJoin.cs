using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Dataflow.Fluent;

namespace Rocks.Dataflow.SplitJoin
{
	public static class DataflowSplitJoin
	{
		/// <summary>
		///     Creates a dataflow block that splits the result to <see cref="SplitJoinItem{TParent, TItem}" /> items
		///     which can be joined afterward with join dataflow block.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<TParent, SplitJoinItem<TParent, TItem>> CreateSplitBlock<TParent, TItem>
			([NotNull] Func<TParent, Task<IReadOnlyList<TItem>>> getItems,
			 ExecutionDataflowBlockOptions options = null)
		{
			if (getItems == null)
				throw new ArgumentNullException ("getItems");

			var result = new TransformManyBlock<TParent, SplitJoinItem<TParent, TItem>>
				(async parent =>
				{
					// ReSharper disable once CompareNonConstrainedGenericWithNull
					if (parent == null)
						return new SplitJoinItem<TParent, TItem>[0];

					try
					{
						var items = await getItems (parent).ConfigureAwait (false);

						var split_join_items = items.Select (item => new SplitJoinItem<TParent, TItem> (parent, item, items.Count))
						                            .ToList ();

						return split_join_items;
					}
					catch (Exception ex)
					{
						var logger = parent as IDataflowErrorLogger;
						if (logger != null)
							logger.OnException (ex);

						return new SplitJoinItem<TParent, TItem>[0];
					}
				},
				 options ?? new ExecutionDataflowBlockOptions ());


			return result;
		}


		/// <summary>
		///     Creates a dataflow block that splits the result to <see cref="SplitJoinItem{TParent, TItem}" /> items
		///     which can be joined afterward with join dataflow block.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<TParent, SplitJoinItem<TParent, TItem>> CreateSplitBlock<TParent, TItem>
			([NotNull] Func<TParent, IReadOnlyList<TItem>> getItems,
			 ExecutionDataflowBlockOptions options = null)
		{
			if (getItems == null)
				throw new ArgumentNullException ("getItems");

			var result = new TransformManyBlock<TParent, SplitJoinItem<TParent, TItem>>
				(parent =>
				{
					// ReSharper disable once CompareNonConstrainedGenericWithNull
					if (parent == null)
						return new SplitJoinItem<TParent, TItem>[0];

					try
					{
						var items = getItems (parent);

						var split_join_items = items.Select (item => new SplitJoinItem<TParent, TItem> (parent, item, items.Count))
						                            .ToList ();

						return split_join_items;
					}
					catch (Exception ex)
					{
						var logger = parent as IDataflowErrorLogger;
						if (logger != null)
							logger.OnException (ex);

						return new SplitJoinItem<TParent, TItem>[0];
					}
				},
				 options ?? new ExecutionDataflowBlockOptions ());

			return result;
		}


		/// <summary>
		///     Creates a dataflow block that process single <see cref="SplitJoinItem{TParent, TItem}" /> item.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<SplitJoinItem<TParent, TItem>, SplitJoinItem<TParent, TItem>> CreateProcessBlock<TParent, TItem>
			([NotNull] Func<TParent, TItem, Task> process,
			 ExecutionDataflowBlockOptions options = null)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			var block = new TransformBlock<SplitJoinItem<TParent, TItem>, SplitJoinItem<TParent, TItem>>
				(async splitJoinItem =>
				{
					if (splitJoinItem == null)
						throw new ArgumentNullException ("splitJoinItem");

					if (splitJoinItem.Result == SplitJoinItemResult.Failure)
						return splitJoinItem;

					splitJoinItem.StartProcessing ();

					try
					{
						await process (splitJoinItem.Parent, splitJoinItem.Item).ConfigureAwait (false);

						splitJoinItem.CompletedSuccessfully ();

						return splitJoinItem;
					}
					catch (Exception ex)
					{
						var logger = splitJoinItem.Item as IDataflowErrorLogger;
						if (logger != null)
							logger.OnException (ex);

						splitJoinItem.Failed (ex);
					}

					return splitJoinItem;
				},
				 options ?? new ExecutionDataflowBlockOptions ());

			return block;
		}


		/// <summary>
		///     Creates a dataflow block that process single <see cref="SplitJoinItem{TParent, TItem}" /> item.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<SplitJoinItem<TParent, TItem>, SplitJoinItem<TParent, TItem>> CreateProcessBlock<TParent, TItem>
			([NotNull] Action<TParent, TItem> process,
			 ExecutionDataflowBlockOptions options = null)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			var block = new TransformBlock<SplitJoinItem<TParent, TItem>, SplitJoinItem<TParent, TItem>>
				(splitJoinItem =>
				{
					if (splitJoinItem == null)
						throw new ArgumentNullException ("splitJoinItem");

					if (splitJoinItem.Result == SplitJoinItemResult.Failure)
						return splitJoinItem;

					splitJoinItem.StartProcessing ();

					try
					{
						process (splitJoinItem.Parent, splitJoinItem.Item);

						splitJoinItem.CompletedSuccessfully ();
					}
					catch (Exception ex)
					{
						var logger = splitJoinItem.Item as IDataflowErrorLogger;
						if (logger != null)
							logger.OnException (ex);

						splitJoinItem.Failed (ex);
					}

					return splitJoinItem;
				},
				 options ?? new ExecutionDataflowBlockOptions ());

			return block;
		}


		/// <summary>
		///     Creates a dataflow block that process single <see cref="SplitJoinItem{TParent, TInputItem}" /> item.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<SplitJoinItem<TParent, TInputItem>, SplitJoinItem<TParent, TOutputItem>>
			CreateProcessBlock<TParent, TInputItem, TOutputItem>
			([NotNull] Func<TParent, TInputItem, Task<TOutputItem>> process,
			 ExecutionDataflowBlockOptions options = null)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			var block = new TransformBlock<SplitJoinItem<TParent, TInputItem>, SplitJoinItem<TParent, TOutputItem>>
				(async splitJoinItem =>
				{
					if (splitJoinItem == null)
						throw new ArgumentNullException ("splitJoinItem");

					if (splitJoinItem.Result == SplitJoinItemResult.Failure)
					{
						var new_split_join_item = new SplitJoinItem<TParent, TOutputItem> (splitJoinItem.Parent,
						                                                                   default (TOutputItem),
						                                                                   splitJoinItem.TotalItemsCount);

						new_split_join_item.Failed (splitJoinItem.Exception);

						return new_split_join_item;
					}

					try
					{
						var item = await process (splitJoinItem.Parent, splitJoinItem.Item).ConfigureAwait (false);

						var new_split_join_item = new SplitJoinItem<TParent, TOutputItem> (splitJoinItem.Parent,
						                                                                   item,
						                                                                   splitJoinItem.TotalItemsCount);

						new_split_join_item.CompletedSuccessfully ();

						return new_split_join_item;
					}
					catch (Exception ex)
					{
						var logger = splitJoinItem.Item as IDataflowErrorLogger;
						if (logger != null)
							logger.OnException (ex);

						var new_split_join_item = new SplitJoinItem<TParent, TOutputItem> (splitJoinItem.Parent,
						                                                                   default (TOutputItem),
						                                                                   splitJoinItem.TotalItemsCount);

						new_split_join_item.Failed (ex);

						return new_split_join_item;
					}
				},
				 options ?? new ExecutionDataflowBlockOptions ());

			return block;
		}


		/// <summary>
		///     Creates a dataflow block that process single <see cref="SplitJoinItem{TParent, TInputItem}" /> item.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<SplitJoinItem<TParent, TInputItem>, SplitJoinItem<TParent, TOutputItem>>
			CreateProcessBlock<TParent, TInputItem, TOutputItem>
			([NotNull] Func<TParent, TInputItem, TOutputItem> process,
			 ExecutionDataflowBlockOptions options = null)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			var block = new TransformBlock<SplitJoinItem<TParent, TInputItem>, SplitJoinItem<TParent, TOutputItem>>
				(splitJoinItem =>
				{
					if (splitJoinItem == null)
						throw new ArgumentNullException ("splitJoinItem");

					if (splitJoinItem.Result == SplitJoinItemResult.Failure)
					{
						var new_split_join_item = new SplitJoinItem<TParent, TOutputItem> (splitJoinItem.Parent,
						                                                                   default (TOutputItem),
						                                                                   splitJoinItem.TotalItemsCount);

						new_split_join_item.Failed (splitJoinItem.Exception);

						return new_split_join_item;
					}

					try
					{
						var item = process (splitJoinItem.Parent, splitJoinItem.Item);

						var new_split_join_item = new SplitJoinItem<TParent, TOutputItem> (splitJoinItem.Parent,
						                                                                   item,
						                                                                   splitJoinItem.TotalItemsCount);

						new_split_join_item.CompletedSuccessfully ();

						return new_split_join_item;
					}
					catch (Exception ex)
					{
						var logger = splitJoinItem.Item as IDataflowErrorLogger;
						if (logger != null)
							logger.OnException (ex);

						var new_split_join_item = new SplitJoinItem<TParent, TOutputItem> (splitJoinItem.Parent,
						                                                                   default (TOutputItem),
						                                                                   splitJoinItem.TotalItemsCount);

						new_split_join_item.Failed (ex);

						return new_split_join_item;
					}
				},
				 options ?? new ExecutionDataflowBlockOptions ());

			return block;
		}


		/// <summary>
		///     Creates a final dataflow block that waits for all <see cref="SplitJoinItem{TParent, TItem}" /> items
		///		to be processed.
		/// </summary>
		[NotNull]
		public static ITargetBlock<SplitJoinItem<TParent, TItem>> CreateFinalJoinBlock<TParent, TItem> ()
		{
			var block = new ActionBlock<SplitJoinItem<TParent, TItem>> (x => { });

			return block;
		}


		/// <summary>
		///     Creates a final dataflow block that waits for all <see cref="SplitJoinItem{TParent, TItem}" /> items
		///		to be processed.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		[NotNull]
		public static ITargetBlock<SplitJoinItem<TParent, TItem>> CreateFinalJoinBlock<TParent, TItem> (
			[NotNull] Func<SplitJoinResult<TParent, TItem>, Task> process)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			var intermediate_results = new Dictionary<TParent, SplitJoinIntermediateResult<TItem>> ();

			var block = new ActionBlock<SplitJoinItem<TParent, TItem>>
				(async x =>
				{
					if (x.Result == null)
						x.CompletedSuccessfully ();

					SplitJoinIntermediateResult<TItem> intermediate_result;
					if (!intermediate_results.TryGetValue (x.Parent, out intermediate_result))
					{
						intermediate_result = new SplitJoinIntermediateResult<TItem> (x.TotalItemsCount);
						intermediate_results[x.Parent] = intermediate_result;
					}

					if (intermediate_result.Completed (x))
					{
						var split_join_result = new SplitJoinResult<TParent, TItem> (x.Parent, intermediate_result);
						await process (split_join_result).ConfigureAwait (false);
					}
				},
				 new ExecutionDataflowBlockOptions
				 {
					 BoundedCapacity = DataflowBlockOptions.Unbounded,
					 MaxDegreeOfParallelism = 1
				 });

			return block;
		}


		/// <summary>
		///     Creates a final dataflow block that waits for all <see cref="SplitJoinItem{TParent, TItem}" /> items
		///		to be processed.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		[NotNull]
		public static ITargetBlock<SplitJoinItem<TParent, TItem>> CreateFinalJoinBlock<TParent, TItem> (
			[NotNull] Action<SplitJoinResult<TParent, TItem>> process)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			var intermediate_results = new Dictionary<TParent, SplitJoinIntermediateResult<TItem>> ();

			var block = new ActionBlock<SplitJoinItem<TParent, TItem>>
				(x =>
				{
					if (x.Result == null)
						x.CompletedSuccessfully ();

					SplitJoinIntermediateResult<TItem> intermediate_result;
					if (!intermediate_results.TryGetValue (x.Parent, out intermediate_result))
					{
						intermediate_result = new SplitJoinIntermediateResult<TItem> (x.TotalItemsCount);
						intermediate_results[x.Parent] = intermediate_result;
					}

					if (intermediate_result.Completed (x))
					{
						var split_join_result = new SplitJoinResult<TParent, TItem> (x.Parent, intermediate_result);
						process (split_join_result);
					}
				},
				 new ExecutionDataflowBlockOptions
				 {
					 BoundedCapacity = DataflowBlockOptions.Unbounded,
					 MaxDegreeOfParallelism = 1
				 });

			return block;
		}


		/// <summary>
		///     Creates a dataflow block that join <see cref="SplitJoinItem{TParent, TItem}" /> items
		///     back into <see cref="SplitJoinResult{TParent, TItem}" /> when all items were proceeded.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<SplitJoinItem<TParent, TItem>, SplitJoinResult<TParent, TItem>> CreateJoinBlock<TParent, TItem> ()
		{
			var intermediate_results = new Dictionary<TParent, SplitJoinIntermediateResult<TItem>> ();

			var block = new TransformManyBlock<SplitJoinItem<TParent, TItem>, SplitJoinResult<TParent, TItem>>
				(x =>
				{
					if (x.Result == null)
						x.CompletedSuccessfully ();

					SplitJoinIntermediateResult<TItem> intermediate_result;
					if (!intermediate_results.TryGetValue (x.Parent, out intermediate_result))
					{
						intermediate_result = new SplitJoinIntermediateResult<TItem> (x.TotalItemsCount);
						intermediate_results[x.Parent] = intermediate_result;
					}

					if (intermediate_result.Completed (x))
					{
						var split_join_result = new SplitJoinResult<TParent, TItem> (x.Parent, intermediate_result);
						return new[] { split_join_result };
					}

					return new SplitJoinResult<TParent, TItem>[0];
				},
				 new ExecutionDataflowBlockOptions
				 {
					 BoundedCapacity = DataflowBlockOptions.Unbounded,
					 MaxDegreeOfParallelism = 1
				 });

			return block;
		}


		/// <summary>
		///     Creates a dataflow block that join <see cref="SplitJoinItem{TParent, TItem}" /> items
		///     back into <see cref="SplitJoinResult{TParent, TItem}" /> when all items were proceeded.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<SplitJoinItem<TParent, TItem>, TOutput> CreateJoinBlock<TParent, TItem, TOutput>
			([NotNull] Func<SplitJoinResult<TParent, TItem>, Task<TOutput>> process)
		{
			if (process == null)
				throw new ArgumentNullException ("process");

			var intermediate_results = new Dictionary<TParent, SplitJoinIntermediateResult<TItem>> ();

			var block = new TransformManyBlock<SplitJoinItem<TParent, TItem>, TOutput>
				(async x =>
				{
					if (x.Result == null)
						x.CompletedSuccessfully ();

					SplitJoinIntermediateResult<TItem> intermediate_result;
					if (!intermediate_results.TryGetValue (x.Parent, out intermediate_result))
					{
						intermediate_result = new SplitJoinIntermediateResult<TItem> (x.TotalItemsCount);
						intermediate_results[x.Parent] = intermediate_result;
					}

					if (intermediate_result.Completed (x))
					{
						var split_join_result = new SplitJoinResult<TParent, TItem> (x.Parent, intermediate_result);
						var result = await process (split_join_result).ConfigureAwait (false);

						return new[] { result };
					}

					return new TOutput[0];
				},
				 new ExecutionDataflowBlockOptions
				 {
					 BoundedCapacity = DataflowBlockOptions.Unbounded,
					 MaxDegreeOfParallelism = 1
				 });

			return block;
		}


		/// <summary>
		///     Creates a dataflow block that join <see cref="SplitJoinItem{TParent, TItem}" /> items
		///     back into <see cref="SplitJoinResult{TParent, TItem}" /> when all items were proceeded.
		///		The <paramref name="process "/> will be called without parallelism and thus can be not thread safe.
		/// </summary>
		[NotNull]
		public static IPropagatorBlock<SplitJoinItem<TParent, TItem>, TOutput> CreateJoinBlock<TParent, TItem, TOutput>
			(Func<SplitJoinResult<TParent, TItem>, TOutput> process)
		{
			var intermediate_results = new Dictionary<TParent, SplitJoinIntermediateResult<TItem>> ();

			var block = new TransformManyBlock<SplitJoinItem<TParent, TItem>, TOutput>
				(x =>
				{
					if (x.Result == null)
						x.CompletedSuccessfully ();

					SplitJoinIntermediateResult<TItem> intermediate_result;
					if (!intermediate_results.TryGetValue (x.Parent, out intermediate_result))
					{
						intermediate_result = new SplitJoinIntermediateResult<TItem> (x.TotalItemsCount);
						intermediate_results[x.Parent] = intermediate_result;
					}

					if (intermediate_result.Completed (x))
					{
						var split_join_result = new SplitJoinResult<TParent, TItem> (x.Parent, intermediate_result);
						var result = process (split_join_result);

						return new[] { result };
					}

					return new TOutput[0];
				},
				 new ExecutionDataflowBlockOptions
				 {
					 BoundedCapacity = DataflowBlockOptions.Unbounded,
					 MaxDegreeOfParallelism = 1
				 });

			return block;
		}
	}
}