using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Tests.FluentTests.Infrastructure;
using Xunit.Abstractions;

namespace Rocks.Dataflow.Tests.FluentTests.CompositionTests
{
    public class FluentAsyncTests
    {
        private readonly ITestOutputHelper output;


        public FluentAsyncTests(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public async Task TransformAction_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<string>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .TransformAsync(async x =>
                                      {
                                          await Task.Yield();
                                          return x.ToString(CultureInfo.InvariantCulture);
                                      })
                      .WithBoundedCapacity(100)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo("1", "2", "3");
        }


        [Fact]
        public async Task TransformManyAction_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<string>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<TestDataflowContext<int>>()
                      .TransformManyAsync<TestDataflowContext<string>>(async x =>
                                                                       {
                                                                           await Task.Yield();
                                                                           return new[] { new TestDataflowContext<string> { Data = x.ToString() } };
                                                                       })
                      .WithBoundedCapacity(100)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x.Data);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }.CreateDataflowContexts()).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo("1", "2", "3");
        }


        [Fact]
        public async Task TransformTransformAction_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<int>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .TransformAsync(async x =>
                                      {
                                          await Task.Yield();
                                          return x.ToString(CultureInfo.InvariantCulture);
                                      })
                      .TransformAsync(async s =>
                                      {
                                          await Task.Yield();
                                          return int.Parse(s);
                                      })
                      .WithBoundedCapacity(100)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo(1, 2, 3);
        }


        [Fact]
        public async Task TransformTransformManyAction_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<char>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .TransformAsync(async x =>
                                      {
                                          await Task.Yield();
                                          return x.ToString(CultureInfo.InvariantCulture);
                                      })
                      .TransformManyAsync<char>(async s =>
                                                {
                                                    await Task.Yield();
                                                    return s.ToCharArray();
                                                })
                      .WithBoundedCapacity(100)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo('1', '2', '3');
        }


        [Fact]
        public async Task TransformAction_WithNullReturn_CorrectlyExecuted()
        {
            // arrange
            var result = new ConcurrentBag<int>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<TestDataflowContext<int>>()
                      .TransformAsync(async x =>
                                      {
                                          await Task.Yield();

                                          if (x.Data == 2)
                                              return null;

                                          return x;
                                      })
                      .WithBoundedCapacity(100)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x.Data);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }.CreateDataflowContexts()).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo(1, 3);
        }


        [Fact]
        public async Task TransformActionSync_WithNullReturn_CorrectlyExecuted()
        {
            // arrange
            var result = new ConcurrentBag<int>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<TestDataflowContext<int>>()
                      .TransformAsync(async x =>
                                      {
                                          await Task.Yield();

                                          if (x.Data == 2)
                                              return null;

                                          return x;
                                      })
                      .WithBoundedCapacity(100)
                      .Action(x => result.Add(x.Data))
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }.CreateDataflowContexts()).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo(1, 3);
        }


        [Fact]
        public async Task ProcessAction_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<int>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .ProcessAsync(async x =>
                                    {
                                        await Task.Yield();
                                    })
                      .WithBoundedCapacity(100)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo(1, 2, 3);
        }


        [Fact]
        public async Task ProcessProcessAction_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<int>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .ProcessAsync(async x =>
                                    {
                                        await Task.Yield();
                                    })
                      .ProcessAsync(async x =>
                                    {
                                        await Task.Yield();
                                    })
                      .WithBoundedCapacity(100)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }).ConfigureAwait(false);


            // assert
            result.Should().BeEquivalentTo(1, 2, 3);
        }


        [Fact]
        public void ActionActionAsync_Throws()
        {
            // arrange

            // act
            Action action = () => DataflowFluent
                                  .ReceiveDataOfType<int>()
                                  .ActionAsync(async x =>
                                               {
                                                   await Task.Yield();
                                               })
                                  .ActionAsync(async x =>
                                               {
                                                   await Task.Yield();
                                               })
                                  .CreateDataflow();


            // assert
            action.Should().Throw<InvalidOperationException>();
        }


        [Fact]
        public void ProcessAction_FirstWaitsForTheSecond_DataflowEndedOnlyAfterBothProcessed()
        {
            // arrange
            var second_item_finished = new SemaphoreSlim(0, 1);
            var completed_items = new ConcurrentBag<string>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<string>()
                      .ProcessAsync(async x =>
                                    {
                                        await Task.Yield();

                                        this.output.WriteLine("ProcessAsync({0})", x);

                                        if (x == "1")
                                            await second_item_finished.WaitAsync();
                                    })
                      .WithMaxDegreeOfParallelism(10)
                      .Action(x =>
                              {
                                  this.output.WriteLine("Action({0})", x);

                                  completed_items.Add(x);

                                  if (x == "2")
                                      second_item_finished.Release();
                              })
                      .CreateDataflow();


            // act
            if (!sut.ProcessAsync(new[] { "1", "2" }).Wait(2000))
                throw new TimeoutException();


            // assert
            completed_items.Should().BeEquivalentTo("2", "1");
        }


        [Fact]
        public async Task StartsWithBatch_WithoutTimeout_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<int[]>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .Batch(3)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            dataflow.Start();

            foreach (var x in Enumerable.Range(1, 2))
                await dataflow.SendAsync(x);

            await Task.Delay(100);

            foreach (var x in Enumerable.Range(3, 3))
                await dataflow.SendAsync(x);

            await dataflow.CompleteAsync();


            // assert
            result.Should().BeEquivalentTo(new[] { 1, 2, 3 }, new[] { 4, 5 });
        }


        [Fact]
        public async Task StartsWithBatch_WithTimeout_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<int[]>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .Batch(5, TimeSpan.FromMilliseconds(100))
                      .WithEnsureOrdered(true)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithEnsureOrdered(true)
                      .WithMaxDegreeOfParallelism(1);


            // act
            var dataflow = sut.CreateDataflow();
            dataflow.Start();

            foreach (var x in Enumerable.Range(1, 2))
                await dataflow.SendAsync(x);

            await Task.Delay(200);

            foreach (var x in Enumerable.Range(3, 3))
                await dataflow.SendAsync(x);

            await dataflow.CompleteAsync();


            // assert
            result.Should().BeEquivalentTo(new[] { 1, 2 }, new[] { 3, 4, 5 });
        }


        [Fact]
        public async Task ContinuesWithBatch_WithoutTimeout_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<int[]>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .Transform(x => x)
                      .Batch(3)
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            dataflow.Start();

            foreach (var x in Enumerable.Range(1, 2))
                await dataflow.SendAsync(x);

            await Task.Delay(100);

            foreach (var x in Enumerable.Range(3, 3))
                await dataflow.SendAsync(x);

            await dataflow.CompleteAsync();


            // assert
            result.Should().BeEquivalentTo(new[] { 1, 2, 3 }, new[] { 4, 5 });
        }


        [Fact]
        public async Task ContinuesStartsWithBatch_WithTimeout_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<int[]>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .Transform(x => x)
                      .Batch(3, TimeSpan.FromMilliseconds(100))
                      .ActionAsync(async x =>
                                   {
                                       await Task.Yield();
                                       result.Add(x);
                                   })
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            dataflow.Start();

            foreach (var x in Enumerable.Range(1, 2))
                await dataflow.SendAsync(x);

            await Task.Delay(200);

            foreach (var x in Enumerable.Range(3, 3))
                await dataflow.SendAsync(x);

            await dataflow.CompleteAsync();


            // assert
            result.Should().BeEquivalentTo(new[] { 1, 2 }, new[] { 3, 4, 5 });
        }
    }
}