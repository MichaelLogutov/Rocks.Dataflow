using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Tests.FluentTests.ExceptionHandlingTests
{
	[TestClass]
	public class FluentSplitJoinAsyncTests
	{
		[TestMethod]
		public async Task SplitTo_WithDefaultExceptionLogger_OneItemsThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async x =>
				{
					await Task.Yield ();

					if (x == "b")
						throw new TestException ();

					return x.ToCharArray ();
				})
				.SplitJoin ()
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


		[TestMethod]
		public async Task SplitProcess_WithDefaultExceptionLogger_OneItemsThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async x =>
				{
					await Task.Yield ();
					return x.ToCharArray ();
				})
				.SplitProcessAsync (async (s, x) =>
				{
					await Task.Yield ();

					if (x == 'b')
						throw new TestException ();
				})
				.SplitJoin ()
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
			failed_items.ShouldAllBeEquivalentTo (new[] { new SplitJoinItem<string, char> ("b", 'b', 1) });
		}


		[TestMethod]
		public async Task SplitTransform_WithDefaultExceptionLogger_OneItemsThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async x =>
				{
					await Task.Yield ();
					return x.ToCharArray ();
				})
				.SplitTransformAsync (async (s, x) =>
				{
					await Task.Yield ();

					if (x == 'b')
						throw new TestException ();

					return x;
				})
				.SplitJoin ()
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
			failed_items.ShouldAllBeEquivalentTo (new[] { new SplitJoinItem<string, char> ("b", 'b', 1) });
		}


		[TestMethod]
		public async Task SplitJoinIntoAction_WithDefaultExceptionLogger_OneItemsThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async x =>
				{
					await Task.Yield ();
					return x.ToCharArray ();
				})
				.SplitJoinIntoAsync (async x =>
				{
					await Task.Yield ();

					if (x.Parent == "b")
						throw new TestException ();

					return x;
				})
				.ActionAsync (async x => { await Task.Yield (); })
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
			failed_items.ShouldAllBeEquivalentTo (new[] { new SplitJoinItem<string, char> ("b", 'b', 1) });
		}


		[TestMethod]
		public async Task SplitJoin_WithDefaultExceptionLogger_OneItemsThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async x =>
				{
					await Task.Yield ();
					return x.ToCharArray ();
				})
				.SplitJoinAsync (async x =>
				{
					await Task.Yield ();

					if (x.Parent == "b")
						throw new TestException ();
				})
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
			failed_items.ShouldAllBeEquivalentTo
				(new[]
				 {
					 new SplitJoinResult<string, char> ("b",
					                                    new char[0],
					                                    new[] { new SplitJoinFailedItem<char> ('b', new TestException ()) },
					                                    1)
				 },
				 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType (x.Expectation.GetType ()))
				                   .WhenTypeIs<Exception> ());
		}


		[TestMethod]
		public async Task SplitTransformJoin_WithDefaultExceptionLogger_OneItemsThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async x =>
				{
					await Task.Yield ();
					return x.ToCharArray ();
				})
				.SplitTransformAsync (async (s, c) =>
				{
					await Task.Yield ();
					return c;
				})
				.SplitJoinAsync (async x =>
				{
					await Task.Yield ();

					if (x.Parent == "b")
						throw new TestException ();
				})
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
			failed_items.ShouldAllBeEquivalentTo
				(new[]
				 {
					 new SplitJoinResult<string, char> ("b",
					                                    new char[0],
					                                    new[] { new SplitJoinFailedItem<char> ('b', new TestException ()) },
					                                    1)
				 },
				 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType (x.Expectation.GetType ()))
				                   .WhenTypeIs<Exception> ());
		}


		[TestMethod]
		public async Task SplitTransformJoinIntoAction_WithDefaultExceptionLogger_OneItemsThrows_LogsTheException ()
		{
			// arrange
			var exceptions = new ConcurrentBag<Exception> ();
			var failed_items = new ConcurrentBag<object> ();

			var sut = DataflowFluent
				.ReceiveDataOfType<string> ()
				.SplitToAsync<char> (async x =>
				{
					await Task.Yield ();
					return x.ToCharArray ();
				})
				.SplitTransformAsync (async (s, c) =>
				{
					await Task.Yield ();
					return c;
				})
				.SplitJoinIntoAsync (async x =>
				{
					await Task.Yield ();

					if (x.Parent == "b")
						throw new TestException ();

					return x;
				})
				.ActionAsync (async x => { await Task.Yield (); })
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
			failed_items.ShouldAllBeEquivalentTo
				(new[]
				 {
					 new SplitJoinResult<string, char> ("b",
					                                    new char[0],
					                                    new[] { new SplitJoinFailedItem<char> ('b', new TestException ()) },
					                                    1)
				 },
				 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType (x.Expectation.GetType ()))
				                   .WhenTypeIs<Exception> ());
		}
	}
}