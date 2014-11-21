using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Dataflow.Extensions;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Tests.CoreTests
{
	[TestClass]
	public class SplitJoinSyncTests
	{
		[TestMethod]
		public async Task SplitJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<string> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s =>
			{
				process.Add (s);
				return s.ToCharArray ();
			});

			var join_block = DataflowSplitJoin.CreateFinalJoinBlock<string, char> ();

			split_block.LinkWithCompletionPropagation (join_block);


			// act
			foreach (var str in new[] { "a", "bb", "ccc" })
				await split_block.SendAsync (str);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, join_block.Completion);


			// assert
			process.Should ().BeEquivalentTo ("a", "bb", "ccc");
		}


		[TestMethod]
		public async Task SplitProcessJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());
			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) => process.Add (c));
			var join_block = DataflowSplitJoin.CreateFinalJoinBlock<string, char> ();

			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (join_block);


			// act
			foreach (var str in new[] { "a", "bb", "ccc" })
				await split_block.SendAsync (str);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, join_block.Completion);


			// assert
			process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
		}


		[TestMethod]
		public async Task SplitProcessJoinAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();
			var process = new ConcurrentBag<char> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());
			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) => process.Add (c));
			var join_block = DataflowSplitJoin.CreateJoinBlock<string, char> ();
			var final_block = new ActionBlock<SplitJoinResult<string, char>> (x => result.Add (new string (x.SucceffullyCompletedItems.ToArray ())));

			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (join_block);
			join_block.LinkWithCompletionPropagation (final_block);


			// act
			foreach (var str in new[] { "a", "bb", "ccc" })
				await split_block.SendAsync (str);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, final_block.Completion);


			// assert
			result.Should ().BeEquivalentTo ("a", "bb", "ccc");
			process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
		}


		[TestMethod]
		public async Task SplitProcessProcessJoinAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();
			var process = new ConcurrentBag<char> ();
			var process2 = new ConcurrentBag<char> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());
			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) => process.Add (c));
			var process_block2 = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) => process2.Add (c));
			var join_block = DataflowSplitJoin.CreateJoinBlock<string, char> ();
			var final_block = new ActionBlock<SplitJoinResult<string, char>> (x => result.Add (new string (x.SucceffullyCompletedItems.ToArray ())));

			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (process_block2);
			process_block2.LinkWithCompletionPropagation (join_block);
			join_block.LinkWithCompletionPropagation (final_block);


			// act
			foreach (var str in new[] { "a", "bb", "ccc" })
				await split_block.SendAsync (str);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, final_block.Completion);


			// assert
			result.Should ().BeEquivalentTo ("a", "bb", "ccc");
			process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
			process2.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
		}


		[TestMethod]
		public async Task SplitProcessTransformJoinAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();
			var process = new ConcurrentBag<char> ();
			var process2 = new ConcurrentBag<char> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());
			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) => process.Add (c));

			var transform_block = DataflowSplitJoin.CreateTransformBlock<string, char, string> ((s, c) =>
			{
				process2.Add (c);
				return c.ToString (CultureInfo.InvariantCulture);
			});

			var join_block = DataflowSplitJoin.CreateJoinBlock<string, string> ();
			var final_block = new ActionBlock<SplitJoinResult<string, string>> (x =>
			{
				foreach (var item in x.SucceffullyCompletedItems)
					result.Add (item);
			});

			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (transform_block);
			transform_block.LinkWithCompletionPropagation (join_block);
			join_block.LinkWithCompletionPropagation (final_block);


			// act
			foreach (var str in new[] { "a", "bb", "ccc" })
				await split_block.SendAsync (str);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, final_block.Completion);


			// assert
			result.Should ().BeEquivalentTo ("a", "b", "b", "c", "c", "c");
			process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
			process2.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
		}


		[TestMethod]
		public async Task SplitProcessJoinAction_ProcessThrowsOnOneItem_ExecutedWithFailedItemHavingTheException ()
		{
			// arrange
			var result = new ConcurrentBag<SplitJoinResult<string, char>> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());

			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) =>
			{
				if (c == 'b')
					throw new TestException ();
			});

			var join_block = DataflowSplitJoin.CreateJoinBlock<string, char> ();
			var final_block = new ActionBlock<SplitJoinResult<string, char>> (x => result.Add (x));


			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (join_block);
			join_block.LinkWithCompletionPropagation (final_block);


			// act
			foreach (var str in new[] { "a", "bc" })
				await split_block.SendAsync (str);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, final_block.Completion);


			// assert
			result.ShouldAllBeEquivalentTo
				(new[]
				 {
					 new SplitJoinResult<string, char> ("a",
					                                    new[] { 'a' },
					                                    new SplitJoinFailedItem<char>[0],
					                                    1),
					 new SplitJoinResult<string, char> ("bc",
					                                    new[] { 'c' },
					                                    new[] { new SplitJoinFailedItem<char> ('b', new TestException ()) },
					                                    2)
				 },
				 options => options.Using<Exception> (x => x.Subject.ShouldBeEquivalentTo (x.Expectation,
				                                                                           o => o.Including (p => p.Message)))
				                   .WhenTypeIs<Exception> ());
		}


		[TestMethod]
		public async Task SplitProcessTransformJoinAction_ProcessThrowsOnOneItem_ExecutedWithFailedItemHavingTheException ()
		{
			// arrange
			var result = new ConcurrentBag<SplitJoinResult<string, string>> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());

			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) =>
			{
				if (c == 'b')
					throw new TestException ();
			});

			var transform_block = DataflowSplitJoin.CreateTransformBlock<string, char, string>
				((s, c) => c.ToString (CultureInfo.InvariantCulture));

			var join_block = DataflowSplitJoin.CreateJoinBlock<string, string> ();
			var final_block = new ActionBlock<SplitJoinResult<string, string>> (x => result.Add (x));


			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (transform_block);
			transform_block.LinkWithCompletionPropagation (join_block);
			join_block.LinkWithCompletionPropagation (final_block);


			// act
			foreach (var str in new[] { "a", "bc" })
				await split_block.SendAsync (str);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, final_block.Completion);


			// assert
			result.ShouldAllBeEquivalentTo
				(new[]
				 {
					 new SplitJoinResult<string, string> ("a",
					                                      new[] { "a" },
					                                      new SplitJoinFailedItem<string>[0],
					                                      1),
					 new SplitJoinResult<string, string> ("bc",
					                                      new[] { "c" },
					                                      new[] { new SplitJoinFailedItem<string> (null, new TestException ()) },
					                                      2)
				 },
				 options => options.Using<Exception> (x => x.Subject.ShouldBeEquivalentTo (x.Expectation,
				                                                                           o => o.Including (p => p.Message)))
				                   .WhenTypeIs<Exception> ());
		}
	}
}