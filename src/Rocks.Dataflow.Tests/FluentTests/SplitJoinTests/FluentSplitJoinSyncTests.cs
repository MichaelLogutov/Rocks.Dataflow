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
	public class FluentSplitJoinSyncTests
	{
		[TestMethod]
		public async Task SplitJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitTo<char> (s =>
				{
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
				.SplitTo<char> (s => s.ToCharArray ())
				.SplitJoinInto (x => new string (x.SucceffullyCompletedItems.ToArray ()))
				.Action (result.Add);


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			result.Should ().BeEquivalentTo ("a", "ab", "abc");
		}


		[TestMethod]
		public async Task SplitTransformJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitTo<char> (s => s.ToCharArray ())
				.SplitTransform ((s, c) =>
				{
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
		public async Task SplitProcessJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitTo (s => s.ToCharArray ())
				.SplitProcess ((s, c) => process.Add (c))
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
				.SplitTo<char> (s => s.ToCharArray ())
				.SplitProcess ((s, c) => process.Add (c))
				.SplitJoinInto (x => new string (x.SucceffullyCompletedItems.ToArray ()))
				.Action (result.Add);


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
				.SplitTo<char> (s => s.ToCharArray ())
				.SplitProcess ((s, c) => process.Add (c))
				.SplitProcess ((s, c) => process2.Add (c))
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
				.SplitTo<char> (s => s.ToCharArray ())
				.SplitProcess ((s, c) => process.Add (c))
				.SplitTransform ((s, c) => (int) c)
				.SplitProcess ((s, n) => process2.Add (n))
				.SplitTransform ((s, n) => (char) n)
				.SplitJoinInto (x => new string (x.SucceffullyCompletedItems.ToArray ()))
				.Action (result.Add);


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ('a', 'a', 'b', 'a', 'b', 'c');
			result.Should ().BeEquivalentTo ("a", "ab", "abc");
		}


		[TestMethod]
		public async Task Split_SplitTransform_Join_Action_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitTo (s => s.ToCharArray ())
				.SplitTransform ((s, c) => (int) c)
				.SplitTransform ((s, i) => (char) i)
				.SplitJoinInto (x => new string (x.SucceffullyCompletedItems.ToArray ()))
				.Action (result.Add);


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
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
				.SplitTo (s => s.ToCharArray ())
				.SplitTransform ((s, c) => (int) c)
				.SplitTransform ((s, i) =>
				{
					var c = (char) i;
					if (c == 'b')
						throw new TestException ();

					return c;
				})
				.SplitJoinInto (x =>
				{
					exceptions.AddRange (x.FailedItems.Select (f => f.Exception));
					return new string (x.SucceffullyCompletedItems.ToArray ());
				})
				.Action (result.Add);


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
				.Transform (s =>
				{
					process.Add (s);
					return s;
				})
				.SplitTo (s =>
				{
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