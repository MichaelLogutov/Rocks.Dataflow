using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Tests.FluentTests.Infrastructure;

namespace Rocks.Dataflow.Tests.FluentTests
{
	[TestClass]
	public class FluentAsyncTests
	{
		[TestMethod]
		public async Task Action_WithException_DoesNotPropagateException ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.Action<string>
				(async x =>
				{
					await Task.Yield ();

					if (x == "b")
						throw new TestException ();

					result.Add (x);
				})
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { "a", "b", "c" });


			// assert
			result.Should ().BeEquivalentTo ("a", "c");
		}


		[TestMethod]
		public async Task Action_WithException_PassTheExceptionToContext ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.Action<TestDataflowContext<string>>
				(async x =>
				{
					await Task.Yield ();

					if (x.Data == "b")
						throw new TestException ();

					result.Add (x.Data);
				})
				.WithMaxDegreeOfParallelism ();

			var contexts = new[] { "a", "b", "c" }.CreateDataflowContexts ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (contexts);


			// assert
			result.Should ().BeEquivalentTo ("a", "c");
			contexts.SelectMany (x => x.Exceptions).Should ().ContainItemsAssignableTo<TestException> ();
		}


		[TestMethod]
		public async Task TransformThenAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.Transform<TestDataflowContext<int>, TestDataflowContext<string>> (async x =>
				{
					await Task.Yield ();
					return new TestDataflowContext<string> { Data = x.ToString () };
				})
				.WithBoundedCapacity (100)
				.Action (async x =>
				{
					await Task.Yield ();
					result.Add (x.Data);
				})
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { 1, 2, 3 }.CreateDataflowContexts ());


			// assert
			result.Should ().BeEquivalentTo ("1", "2", "3");
		}


		[TestMethod]
		public async Task TransformManyThenAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.TransformMany<TestDataflowContext<int>, TestDataflowContext<string>> (async x =>
				{
					await Task.Yield ();
					return new[] { new TestDataflowContext<string> { Data = x.ToString () } };
				})
				.WithBoundedCapacity (100)
				.Action (async x =>
				{
					await Task.Yield ();
					result.Add (x.Data);
				})
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { 1, 2, 3 }.CreateDataflowContexts ());


			// assert
			result.Should ().BeEquivalentTo ("1", "2", "3");
		}
	}
}