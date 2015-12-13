using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Tests.FluentTests.Infrastructure;

namespace Rocks.Dataflow.Tests.FluentTests.ExceptionHandlingTests
{
	public class FluentActionAsyncTests
	{
		[Fact]
		public async Task WithItemImplementingLogger_OneItemThrows_PassTheExceptionToContext ()
		{
			// arrange
			var sut = DataflowFluent
				.ReceiveDataOfType<TestDataflowContext<string>> ()
				.ActionAsync (async x =>
				{
					await Task.Yield ();

					if (x.Data == "b")
						throw new TestException ();
				})
				.WithMaxDegreeOfParallelism ();

			var contexts = new[] { "a", "b", "c" }.CreateDataflowContexts ();


			// act
			var dataflow = sut.CreateDataflow ();
			await dataflow.ProcessAsync (contexts);


			// assert
			contexts.SelectMany (x => x.Exceptions).Should ().HaveCount (1).And.ContainItemsAssignableTo<TestException> ();
		}


		[Fact]
		public async Task WithDefaultExceptionLogger_OneItemThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.ActionAsync (async x =>
				{
					await Task.Yield ();

					if (x == "b")
						throw new TestException ();
				})
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


		[Fact]
		public async Task WithDefaultExceptionLoggerSetInTheMiddle_OneItemThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.TransformAsync (async x =>
				{
					await Task.Yield ();
					return x;
				})
				.WithDefaultExceptionLogger ((exception, item) =>
				{
					exceptions.Add (exception);
					failed_items.Add (item);
				})
				.ActionAsync (async x =>
				{
					if (x == "b")
						throw new TestException ();

					await Task.Yield ();
				})
				.WithMaxDegreeOfParallelism ()
				.CreateDataflow ();


			// act
			await sut.ProcessAsync (new[] { "a", "b", "c" });


			// assert
			exceptions.Should ().HaveCount (1).And.ContainItemsAssignableTo<TestException> ();
			failed_items.Should ().Equal ("b");
		}
	}
}


