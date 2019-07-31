using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Tests.FluentTests.Infrastructure;
using Xunit;

namespace Rocks.Dataflow.Tests.FluentTests.CompositionTests
{
    public class FluentSyncTests
    {
        [Fact]
        public async Task TransformAction_CorrectlyBuild()
        {
            // arrange
            var result = new ConcurrentBag<string>();

            var sut = DataflowFluent
                      .ReceiveDataOfType<int>()
                      .Transform(x => x.ToString(CultureInfo.InvariantCulture))
                      .WithBoundedCapacity(100)
                      .Action(result.Add)
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 });


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
                      .TransformMany(x => new[] { new TestDataflowContext<string> { Data = x.ToString() } })
                      .WithBoundedCapacity(100)
                      .Action(x => result.Add(x.Data))
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }.CreateDataflowContexts());


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
                      .Transform(x => x.ToString(CultureInfo.InvariantCulture))
                      .Transform(int.Parse)
                      .WithBoundedCapacity(100)
                      .Action(result.Add)
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 });


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
                      .Transform(x => x.ToString(CultureInfo.InvariantCulture))
                      .TransformMany(s => s.ToCharArray())
                      .WithBoundedCapacity(100)
                      .Action(result.Add)
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 });


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
                      .Transform(x => x.Data == 2 ? null : x)
                      .WithBoundedCapacity(100)
                      .Action(x => result.Add(x.Data))
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 }.CreateDataflowContexts());


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
                      .Process(x =>
                               {
                               })
                      .WithBoundedCapacity(100)
                      .Action(result.Add)
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 });


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
                      .Process(x =>
                               {
                               })
                      .Process(x =>
                               {
                               })
                      .WithBoundedCapacity(100)
                      .Action(result.Add)
                      .WithMaxDegreeOfParallelism();


            // act
            var dataflow = sut.CreateDataflow();
            await dataflow.ProcessAsync(new[] { 1, 2, 3 });


            // assert
            result.Should().BeEquivalentTo(1, 2, 3);
        }


        [Fact]
        public void ActionAction_Throws()
        {
            // arrange

            // act
            Action action = () => DataflowFluent
                                  .ReceiveDataOfType<int>()
                                  .Action(x =>
                                          {
                                          })
                                  .Action(x =>
                                          {
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

            var starting_block = new TransformBlock<string, string>(x =>
                                                                    {
                                                                        if (x == "1")
                                                                            second_item_finished.Wait();

                                                                        return x;
                                                                    },
                                                                    new ExecutionDataflowBlockOptions
                                                                    {
                                                                        BoundedCapacity = 100,
                                                                        MaxDegreeOfParallelism = 4,
                                                                        EnsureOrdered = false
                                                                    });

            var ending_block = new ActionBlock<string>(x =>
                                                       {
                                                           completed_items.Add(x);

                                                           if (x == "2")
                                                               second_item_finished.Release();
                                                       },
                                                       new ExecutionDataflowBlockOptions
                                                       {
                                                           BoundedCapacity = 100,
                                                           MaxDegreeOfParallelism = 4,
                                                           EnsureOrdered = false
                                                       });

            starting_block.LinkTo(ending_block, new DataflowLinkOptions { PropagateCompletion = true });


            // act
            foreach (var data in new[] { "1", "2" })
            {
                if (!starting_block.SendAsync(data).Wait(1000))
                    throw new TimeoutException();
            }

            starting_block.Complete();

            if (!Task.WaitAll(new[] { starting_block.Completion, ending_block.Completion }, 2000))
                throw new TimeoutException();


            // assert
            completed_items.Should().BeEquivalentTo("2", "1");
        }
    }
}