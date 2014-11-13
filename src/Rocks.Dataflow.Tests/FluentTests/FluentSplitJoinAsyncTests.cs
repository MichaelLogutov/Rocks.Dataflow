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
	public class FluentSplitJoinAsyncTests
	{
		[TestMethod]
		public async Task SplitJoin_CorrectlyBuilded ()
		{
			// arrange
			var process = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.Split<string, char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitProcess (async (s, c) =>
				{
					await Task.Yield ();
					process.Add (c);
				})
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
				.Split<string, char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitProcess (async (s, c) =>
				{
					await Task.Yield ();
					process.Add (c);
				})
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
				.Split<string, char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitTransform (async (s, c) =>
				{
					await Task.Yield ();
					return (int) c;
				})
				.SplitTransform (async (s, i) =>
				{
					await Task.Yield ();
					return (char) i;
				})
				.Join (async x =>
				{
					await Task.Yield ();
					return new string (x.SucceffullyCompletedItems.ToArray ());
				})
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
				.Split<string, char> (async s =>
				{
					await Task.Yield ();
					return s.ToCharArray ();
				})
				.SplitTransform (async (s, c) =>
				{
					await Task.Yield ();
					return (int) c;
				})
				.SplitTransform (async (s, i) =>
				{
					await Task.Yield ();

					var c = (char) i;
					if (c == 'b')
						throw new TestException ();

					return c;
				})
				.Join (async x =>
				{
					await Task.Yield ();

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