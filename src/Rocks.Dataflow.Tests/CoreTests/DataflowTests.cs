using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Rocks.Dataflow.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Rocks.Dataflow.Tests.CoreTests
{
    public class DataflowTests
    {
        private readonly ITestOutputHelper output;


        public DataflowTests(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public async Task TwoItemsProcessed_FirstWaitsForTheSecond_DataflowEndedOnlyAfterBothProcessed()
        {
            // arrange
            var second_item_finished = new SemaphoreSlim(0, 1);
            var completed_items = new ConcurrentBag<string>();

            var start_block = new BufferBlock<string>(new DataflowBlockOptions
                                                      {
                                                          BoundedCapacity = 10,
                                                          EnsureOrdered = false
                                                      });

            var process_block = new TransformBlock<string, string>(
                async x =>
                {
                    await Task.Yield();

                    this.output.WriteLine("Process block: {0}", x);

                    if (x == "1")
                        await second_item_finished.WaitAsync();

                    return x;
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 4,
                    BoundedCapacity = 10,
                    EnsureOrdered = false
                });


            var final_block = new ActionBlock<string>(
                x =>
                {
                    this.output.WriteLine("Final block: {0}", x);

                    if (x == "2")
                        second_item_finished.Release();

                    completed_items.Add(x);
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 4,
                    BoundedCapacity = 10,
                    EnsureOrdered = false
                });

            start_block.LinkTo(process_block, new DataflowLinkOptions { PropagateCompletion = true });
            process_block.LinkTo(final_block, new DataflowLinkOptions { PropagateCompletion = true });


            // act
            foreach (var item in new[] { "1", "2" })
                if (!await start_block.SendAsync(item))
                    this.output.WriteLine("Unable to send: {0}", item);

            start_block.Complete();

            if (!final_block.Completion.Wait(2000))
                throw new TimeoutException();


            // assert
            completed_items.Should().BeEquivalentTo("1", "2");
        }


        [Fact]
        public void EndsWithSourceBlock_Throws()
        {
            // arrange
            var starting_block = new TransformBlock<string, string>(s => s);
            var final_block = new TransformBlock<string, string>(s => s);

            starting_block.LinkWithCompletionPropagation(final_block);


            // act
            Action action = () => new Dataflow<string>(starting_block, final_block);


            // assert
            action.Should().Throw<ArgumentException>();
        }


        [Fact]
        public async Task ActionBlock_CorrectlyProceed()
        {
            // arrange
            var result = new ConcurrentQueue<string>();

            var block = new ActionBlock<string>(s => result.Enqueue(s));

            var sut = new Dataflow<string>(block, block);


            // act
            await sut.ProcessAsync(new[] { "a", "b", "c" });


            // assert
            result.Should().BeEquivalentTo("a", "b", "c");
        }


        [Fact]
        public async Task ActionThenTransformBlock_CorrectlyProceed()
        {
            // arrange
            var result = new ConcurrentQueue<string>();

            var starting_block = new TransformBlock<string, string>(s => s);
            var final_block = new ActionBlock<string>(s => result.Enqueue(s));

            starting_block.LinkWithCompletionPropagation(final_block);

            var sut = new Dataflow<string>(starting_block, final_block);


            // act
            await sut.ProcessAsync(new[] { "a", "b", "c" });


            // assert
            result.Should().BeEquivalentTo("a", "b", "c");
        }
    }
}