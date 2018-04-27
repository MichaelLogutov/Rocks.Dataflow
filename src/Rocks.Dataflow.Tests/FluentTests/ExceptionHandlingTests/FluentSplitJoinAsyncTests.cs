using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.SplitJoin;
using Xunit;

namespace Rocks.Dataflow.Tests.FluentTests.ExceptionHandlingTests
{
    public class FluentSplitJoinAsyncTests
    {
        [Fact]
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


        [Fact]
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

            var expected_failed_item = new SplitJoinItem<string, char> ("b", 'b', 1);
            expected_failed_item.Failed (new TestException ());

            failed_items.Should().BeEquivalentTo
                (new[] { expected_failed_item },
                 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType<TestException> ())
                                   .WhenTypeIs<Exception> ());
        }


        [Fact]
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
            failed_items.Should().BeEquivalentTo (new[] { new SplitJoinItem<string, char> ("b", 'b', 1) });
        }


        [Fact]
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
            failed_items.Should().BeEquivalentTo (new[]
                                                  {
                                                      new SplitJoinResult<string, char> ("b",
                                                                                         new[] { 'b' },
                                                                                         new SplitJoinFailedItem<char>[0],
                                                                                         1)
                                                  });
        }


        [Fact]
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
            failed_items.Should().BeEquivalentTo
                (new[]
                 {
                     new SplitJoinResult<string, char> ("b",
                                                        new[] { 'b' },
                                                        new SplitJoinFailedItem<char>[0],
                                                        1)
                 },
                 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType (x.Expectation.GetType ()))
                                   .WhenTypeIs<Exception> ());
        }


        [Fact]
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
            failed_items.Should().BeEquivalentTo
                (new[]
                 {
                     new SplitJoinResult<string, char> ("b",
                                                        new[] { 'b' },
                                                        new SplitJoinFailedItem<char>[0],
                                                        1)
                 },
                 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType (x.Expectation.GetType ()))
                                   .WhenTypeIs<Exception> ());
        }


        [Fact]
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
            failed_items.Should().BeEquivalentTo
                (new[]
                 {
                     new SplitJoinResult<string, char> ("b",
                                                        new[] { 'b' },
                                                        new SplitJoinFailedItem<char>[0],
                                                        1)
                 },
                 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType (x.Expectation.GetType ()))
                                   .WhenTypeIs<Exception> ());
        }


        [Fact]
        public void SplitProcessJoinAction_TwoItems_SecondWaitsForTheFirst_DataflowEndedOnlyAfterBothProcessed ()
        {
            // arrange
            var first_item_finished = new SemaphoreSlim (0, 1);
            var completed_items = new ConcurrentBag<string> ();

            var sut = DataflowFluent
                .ReceiveDataOfType<string> ()
                .SplitToAsync<char> (async x =>
                                           {
                                               await Task.Yield ();
                                               return x.ToCharArray ();
                                           })
                .SplitProcessAsync (async (s, c) =>
                                          {
                                              await Task.Yield ();

                                              if (c == '1')
                                                  await Task.Delay (100);
                                              else if (c == '2')
                                                  await first_item_finished.WaitAsync ();
                                          })
                .SplitJoinInto (result => result.Parent)
                .ActionAsync (async x =>
                                    {
                                        await Task.Yield ();
                                        if (x == "111")
                                            first_item_finished.Release ();
                                        completed_items.Add (x);
                                    })
                .CreateDataflow ();


            // act
            if (!sut.ProcessAsync (new[] { "111", "2" }).Wait (1000))
                throw new TimeoutException ();


            // assert
            completed_items.Should ().BeEquivalentTo ("111", "2");
        }
    }
}