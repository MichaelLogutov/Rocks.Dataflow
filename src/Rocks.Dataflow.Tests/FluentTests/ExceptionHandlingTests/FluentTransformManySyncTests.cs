using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Rocks.Dataflow.Fluent;

namespace Rocks.Dataflow.Tests.FluentTests.ExceptionHandlingTests
{
	public class FluentTransformManySyncTests
	{
		[Fact]
		public async Task OneItemThrows_ProceedTheRest ()
		{
			// arrange
			var result = new ConcurrentBag<string> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.TransformMany<string> (x =>
				{
					if (x == "b")
						throw new TestException ();

					return new[] { x };
				})
				.Action (result.Add)
				.WithMaxDegreeOfParallelism ()
				.CreateDataflow ();


			// act
			await sut.ProcessAsync (new[] { "a", "b", "c" });


			// assert
			result.Should ().BeEquivalentTo ("a", "c");
		}


		[Fact]
		public async Task WithDefaultExceptionLogger_OneItemThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.TransformMany<string> (x =>
				{
					if (x == "b")
						throw new TestException ();

					return new[] { x };
				})
				.Action (x => { })
				.WithMaxDegreeOfParallelism ()
				.WithDefaultExceptionLogger ((exception, item) =>
				{
					exceptions.Add (exception);
					failed_items.Add (item);
				})
				.CreateDataflow ();


			// act
			await sut.ProcessAsync (new[] { "a", "b", "c" });


			// assert
			exceptions.Should ().HaveCount (1).And.ContainItemsAssignableTo<TestException> ();
			failed_items.Should ().Equal ("b");
		}
	}
}


