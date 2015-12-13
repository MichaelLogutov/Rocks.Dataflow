using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Tests.FluentTests.Infrastructure;

namespace Rocks.Dataflow.Tests.FluentTests.CompositionTests
{
    public class FluentSyncTests
    {
        [Fact]
        public async Task TransformAction_CorrectlyBuild ()
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
            await dataflow.ProcessAsync (new[] { 1, 2, 3 });


            // assert
            result.Should ().BeEquivalentTo ("1", "2", "3");
        }


        [Fact]
        public async Task TransformManyAction_CorrectlyBuild ()
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
            await dataflow.ProcessAsync (new[] { 1, 2, 3 }.CreateDataflowContexts ());


            // assert
            result.Should ().BeEquivalentTo ("1", "2", "3");
        }


        [Fact]
        public async Task TransformTransformAction_CorrectlyBuild ()
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
            await dataflow.ProcessAsync (new[] { 1, 2, 3 });


            // assert
            result.Should ().BeEquivalentTo (1, 2, 3);
        }


        [Fact]
        public async Task TransformTransformManyAction_CorrectlyBuild ()
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
            await dataflow.ProcessAsync (new[] { 1, 2, 3 });


            // assert
            result.Should ().BeEquivalentTo ('1', '2', '3');
        }


        [Fact]
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
            await dataflow.ProcessAsync (new[] { 1, 2, 3 }.CreateDataflowContexts ());


            // assert
            result.Should ().BeEquivalentTo (1, 3);
        }


        [Fact]
        public async Task ProcessAction_CorrectlyBuild ()
        {
            // arrange
            var result = new ConcurrentBag<int> ();

            var sut = DataflowFluent
                .ReceiveDataOfType<int> ()
                .Process (x => { })
                .WithBoundedCapacity (100)
                .Action (result.Add)
                .WithMaxDegreeOfParallelism ();


            // act
            var dataflow = sut.CreateDataflow ();
            await dataflow.ProcessAsync (new[] { 1, 2, 3 });


            // assert
            result.Should ().BeEquivalentTo (1, 2, 3);
        }


        [Fact]
        public async Task ProcessProcessAction_CorrectlyBuild ()
        {
            // arrange
            var result = new ConcurrentBag<int> ();

            var sut = DataflowFluent
                .ReceiveDataOfType<int> ()
                .Process (x => { })
                .Process (x => { })
                .WithBoundedCapacity (100)
                .Action (result.Add)
                .WithMaxDegreeOfParallelism ();


            // act
            var dataflow = sut.CreateDataflow ();
            await dataflow.ProcessAsync (new[] { 1, 2, 3 });


            // assert
            result.Should ().BeEquivalentTo (1, 2, 3);
        }


        [Fact]
        public void ActionAction_Throws ()
        {
            // arrange

            // act
            Action action = () => DataflowFluent
                                      .ReceiveDataOfType<int> ()
                                      .Action (x => { })
                                      .Action (x => { })
                                      .CreateDataflow ();


            // assert
            action.ShouldThrow<InvalidOperationException> ();
        }
    }
}


