using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Tests.FluentTests.Infrastructure;

namespace Rocks.Dataflow.Tests.FluentTests.CompositionTests
{
	[TestClass]
	public class FluentSyncTests
	{
		[TestMethod]
		public async Task TransformAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<int> ()
				.Transform (x => x.ToString (CultureInfo.InvariantCulture))
				.WithBoundedCapacity (100)
				.Action (result.Add)
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { 1, 2, 3 });


			// assert
			result.Should ().BeEquivalentTo ("1", "2", "3");
		}


		[TestMethod]
		public async Task TransformManyAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<TestDataflowContext<int>> ()
				.TransformMany (x => new[] { new TestDataflowContext<string> { Data = x.ToString () } })
				.WithBoundedCapacity (100)
				.Action (x => result.Add (x.Data))
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { 1, 2, 3 }.CreateDataflowContexts ());


			// assert
			result.Should ().BeEquivalentTo ("1", "2", "3");
		}


		[TestMethod]
		public async Task TransformTransformAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<int> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<int> ()
				.Transform (x => x.ToString (CultureInfo.InvariantCulture))
				.Transform (int.Parse)
				.WithBoundedCapacity (100)
				.Action (result.Add)
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { 1, 2, 3 });


			// assert
			result.Should ().BeEquivalentTo (1, 2, 3);
		}


		[TestMethod]
		public async Task TransformTransformManyAction_CorrectlyBuilded ()
		{
			// arrange
			var result = new ConcurrentBag<char> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<int> ()
				.Transform (x => x.ToString (CultureInfo.InvariantCulture))
				.TransformMany (s => s.ToCharArray ())
				.WithBoundedCapacity (100)
				.Action (result.Add)
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { 1, 2, 3 });


			// assert
			result.Should ().BeEquivalentTo ('1', '2', '3');
		}


		[TestMethod]
		public async Task TransformAction_WithNullReturn_CorrectlyExecuted ()
		{
			// arrange
			var result = new ConcurrentBag<int> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<TestDataflowContext<int>> ()
				.Transform (x => x.Data == 2 ? null : x)
				.WithBoundedCapacity (100)
				.Action (x => result.Add (x.Data))
				.WithMaxDegreeOfParallelism ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.Process (new[] { 1, 2, 3 }.CreateDataflowContexts ());


			// assert
			result.Should ().BeEquivalentTo (1, 3);
		}
	}
}