using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Xunit;
using Rocks.Dataflow.Extensions;

namespace Rocks.Dataflow.Tests.CoreTests
{
	public class DataflowTests
	{
		[Fact]
		public void EndsWithSourceBlock_Throws ()
		{
			// arrange
			var starting_block = new TransformBlock<string, string> (s => s);
			var final_block = new TransformBlock<string, string> (s => s);

			starting_block.LinkWithCompletionPropagation (final_block);


			// act
			Action action = () => new Dataflow<string> (starting_block, final_block);


			// assert
			action.ShouldThrow<ArgumentException> ();
		}


		[Fact]
		public async Task ActionBlock_CorrectlyProceed ()
		{
			// arrange
			var result = new ConcurrentQueue<string> ();

			var block = new ActionBlock<string> (s => result.Enqueue (s));

			var sut = new Dataflow<string> (block, block);


			// act
			await sut.ProcessAsync (new[] { "a", "b", "c" });


			// assert
			result.Should ().BeEquivalentTo ("a", "b", "c");
		}


		[Fact]
		public async Task ActionThenTransformBlock_CorrectlyProceed ()
		{
			// arrange
			var result = new ConcurrentQueue<string> ();

			var starting_block = new TransformBlock<string, string> (s => s);
			var final_block = new ActionBlock<string> (s => result.Enqueue (s));

			starting_block.LinkWithCompletionPropagation (final_block);

			var sut = new Dataflow<string> (starting_block, final_block);


			// act
			await sut.ProcessAsync (new[] { "a", "b", "c" });


			// assert
			result.Should ().BeEquivalentTo ("a", "b", "c");
		}
	}
}


