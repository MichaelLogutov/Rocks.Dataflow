using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Dataflow.Fluent;

namespace Rocks.Dataflow.Tests.FluentTests
{
	[TestClass]
	public class FluentSplitJoinSyncTests
	{
		[TestMethod]
		public async Task SplitJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.Split<string, char> (s => s.ToCharArray ())
				.SplitProcess ((s, c) => process.Add (c))
				.Join ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			process.Should ().BeEquivalentTo ('a', 'a', 'b', 'a', 'b', 'c');
		}


		[TestMethod]
		public async Task SplitJoinAction_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.Split<string, char> (s => s.ToCharArray ())
				.SplitProcess ((s, c) => process.Add (c))
				.Join (x => new string (x.SucceffullyCompletedItems.ToArray ()))
				.Action (s => result.Add (s));


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
				.Split<string, char> (s => s.ToCharArray ())
				.SplitTransform ((s, c) => (int) c)
				.SplitTransform ((s, i) => (char) i)
				.Join (x => new string (x.SucceffullyCompletedItems.ToArray ()))
				.Action (s => result.Add (s));


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
				.Split<string, char> (s => s.ToCharArray ())
				.SplitTransform ((s, c) => (int) c)
				.SplitTransform ((s, i) =>
				{
					var c = (char) i;
					if (c == 'b')
						throw new TestException ();

					return c;
				})
				.Join (x =>
				{
					exceptions.AddRange (x.FailedItems.Select (f => f.Exception));
					return new string (x.SucceffullyCompletedItems.ToArray ());
				})
				.Action (s => result.Add (s));


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "ab", "abc" });


			// assert
			result.Should ().BeEquivalentTo ("a", "a", "ac");
			exceptions.Should ().ContainItemsAssignableTo<TestException> ();
		}
	}
}