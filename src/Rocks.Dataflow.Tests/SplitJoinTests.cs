using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Dataflow.Extensions;
using Rocks.Dataflow.SplitJoin;
using Rocks.Dataflow.Tests.FluentTests.Infrastructure;

namespace Rocks.Dataflow.Tests
{
	[TestClass]
	public class SplitJoinTests
	{
		[TestMethod]
		public async Task SplitProcessJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (async s =>
			{
				await Task.Yield ();
				return s.ToCharArray ();
			});

			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> (async (s, c) =>
			{
				await Task.Yield ();
				process.Add (c);
				await Task.Yield ();
			});

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
		public async Task SplitProcessJoinAction_FaultOnSplit_CorrectlyExecuted ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();
			var process = new ConcurrentBag<char> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock (new Func<TestDataflowContext<string>, Task<IReadOnlyList<char>>>
				                                                      (s => { throw new TestException (); }));

			var process_block = DataflowSplitJoin.CreateProcessBlock<TestDataflowContext<string>, char> ((s, c) => process.Add (c));
			var join_block = DataflowSplitJoin.CreateJoinBlock<TestDataflowContext<string>, char> ();
			var final_block = new ActionBlock<SplitJoinResult<TestDataflowContext<string>, char>>
				(x => result.Add (new string (x.SucceffullyCompletedItems.ToArray ())));

			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (join_block);
			join_block.LinkWithCompletionPropagation (final_block);


			// act
			var contexts = new[] { "a", "bb", "ccc" }.CreateDataflowContexts ();

			foreach (var context in contexts)
				await split_block.SendAsync (context);

			split_block.Complete ();
			await Task.WhenAll (split_block.Completion, final_block.Completion);


			// assert
			result.Should ().BeEmpty ();
			process.Should ().BeEmpty ();
			contexts.Select (x => x.Exceptions).Should ().HaveCount (3);
			contexts.Select (x => x.Exceptions).ElementAt (0).Should ().HaveCount (1);
			contexts.Select (x => x.Exceptions).ElementAt (0).ElementAt (0).Should ().BeOfType<TestException> ();
			contexts.Select (x => x.Exceptions).ElementAt (1).Should ().HaveCount (1);
			contexts.Select (x => x.Exceptions).ElementAt (1).ElementAt (0).Should ().BeOfType<TestException> ();
			contexts.Select (x => x.Exceptions).ElementAt (2).Should ().HaveCount (1);
			contexts.Select (x => x.Exceptions).ElementAt (2).ElementAt (0).Should ().BeOfType<TestException> ();
		}


		[TestMethod]
		public async Task TwoProcess_CorrectlyBuilded ()
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
		public async Task WithTransformProcess_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();
			var process = new ConcurrentBag<char> ();
			var process2 = new ConcurrentBag<char> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());
			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) => process.Add (c));

			var process_block2 = DataflowSplitJoin.CreateProcessBlock<string, char, string> ((s, c) =>
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
			process_block.LinkWithCompletionPropagation (process_block2);
			process_block2.LinkWithCompletionPropagation (join_block);
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
		public async Task ProcessWithFail_CorrectlyBuilded ()
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
		public async Task WithTransformProcess_WithFail_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<SplitJoinResult<string, string>> ();

			var split_block = DataflowSplitJoin.CreateSplitBlock<string, char> (s => s.ToCharArray ());
			var process_block = DataflowSplitJoin.CreateProcessBlock<string, char> ((s, c) =>
			{
				if (c == 'b')
					throw new TestException ();
			});

			var process_block2 = DataflowSplitJoin.CreateProcessBlock
				(new Func<string, char, string> ((s, c) => c.ToString (CultureInfo.InvariantCulture)));

			var join_block = DataflowSplitJoin.CreateJoinBlock<string, string> ();
			var final_block = new ActionBlock<SplitJoinResult<string, string>> (x => result.Add (x));


			split_block.LinkWithCompletionPropagation (process_block);
			process_block.LinkWithCompletionPropagation (process_block2);
			process_block2.LinkWithCompletionPropagation (join_block);
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