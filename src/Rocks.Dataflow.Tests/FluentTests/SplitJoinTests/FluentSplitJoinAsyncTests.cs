using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Dataflow.Fluent;

namespace Rocks.Dataflow.Tests.FluentTests.SplitJoinTests
{
	[TestClass]
	public class FluentSplitJoinAsyncTests
	{
		[TestMethod]
		public async Task SplitJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					process.Add (s);
					return s.ToCharArray ();
				})
				.SplitJoin ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ("a", "ab", "abc");
		}


		[TestMethod]
		public async Task SplitJoinInto_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitJoinIntoAsync (async x =>
				{
					await Task.Yield ();
					return new string (x.SucceffullyCompletedItems.ToArray ());
				})
				.DoAsync (async x =>
				{
					await Task.Yield ();
					result.Add (x);
				});


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			result.Should ().BeEquivalentTo ("a", "ab", "abc");
		}


		[TestMethod]
		public async Task SplitProcessJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitProcessAsync (async (s, c) =>
				{
					await Task.Yield ();
					process.Add (c);
				})
				.SplitJoin ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ('a', 'a', 'b', 'a', 'b', 'c');
		}


		[TestMethod]
		public async Task SplitTransformJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitTransformAsync (async (s, c) =>
				{
					await Task.Yield ();
					process.Add (c);
					return (int) c;
				})
				.SplitJoin ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ('a', 'a', 'b', 'a', 'b', 'c');
		}


		[TestMethod]
		public async Task SplitProcessJoinInto_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitProcessAsync (async (s, c) =>
				{
					await Task.Yield ();
					process.Add (c);
				})
				.SplitJoinIntoAsync (async x =>
				{
					await Task.Yield ();
					return new string (x.SucceffullyCompletedItems.ToArray ());
				})
				.DoAsync (async x =>
				{
					await Task.Yield ();
					result.Add (x);
				});


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ('a', 'a', 'b', 'a', 'b', 'c');
			result.Should ().BeEquivalentTo ("a", "ab", "abc");
		}


		[TestMethod]
		public async Task SplitProcessProcessJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();
			var process2 = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitProcessAsync (async (s, c) =>
				{
					await Task.Yield ();
					process.Add (c);
				})
				.SplitProcessAsync (async (s, c) =>
				{
					await Task.Yield ();
					process2.Add (c);
				})
				.SplitJoin ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ('a', 'a', 'b', 'a', 'b', 'c');
			process.Should ().BeEquivalentTo (process2);
		}


		[TestMethod]
		public async Task SplitJoinWithAllBlockTypes_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();
			var process = new ConcurrentBag<char> ();
			var process2 = new ConcurrentBag<int> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitProcessAsync (async (s, c) =>
				{
					await Task.Yield ();
					process.Add (c);
				})
				.SplitTransformAsync (async (s, c) =>
				{
					await Task.Yield ();
					return (int) c;
				})
				.SplitProcessAsync (async (s, n) =>
				{
					await Task.Yield ();
					process2.Add (n);
				})
				.SplitTransformAsync (async (s, n) =>
				{
					await Task.Yield ();
					return (char) n;
				})
				.SplitJoinIntoAsync (async x =>
				{
					await Task.Yield ();
					return new string (x.SucceffullyCompletedItems.ToArray ());
				})
				.DoAsync (async x =>
				{
					await Task.Yield ();
					result.Add (x);
				});


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ('a', 'a', 'b', 'a', 'b', 'c');
			result.Should ().BeEquivalentTo ("a", "ab", "abc");
		}


		[TestMethod]
		public async Task Split_SplitTransform_Join_Action_WithFailedItems_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();
			var exceptions = new List<Exception> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitTransformAsync (async (s, c) =>
				{
					await Task.Yield ();
					return (int) c;
				})
				.SplitTransformAsync (async (s, i) =>
				{
					await Task.Yield ();

					var c = (char) i;
					if (c == 'b')
						throw new TestException ();

					return c;
				})
				.SplitJoinIntoAsync (async x =>
				{
					await Task.Yield ();

					exceptions.AddRange (x.FailedItems.Select (f => f.Exception));
					return new string (x.SucceffullyCompletedItems.ToArray ());
				})
				.DoAsync (async x =>
				{
					await Task.Yield ();
					result.Add (x);
				});


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			result.Should ().BeEquivalentTo ("a", "a", "ac");
			exceptions.Should ().ContainItemsAssignableTo<TestException> ();
		}


		[TestMethod]
		public async Task TransformSplitJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<string> ();
			var process2 = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.TransformAsync (async s =>
				{
					await Task.Yield ();
					process.Add (s);
					return s;
				})
				.SplitToAsync<char> (async s =>
				{
					await Task.Yield ();
					process2.Add (s);
					return s.ToCharArray ();
				})
				.SplitJoin ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ("a", "ab", "abc");
			process.Should ().BeEquivalentTo (process2);
		}
	}
}