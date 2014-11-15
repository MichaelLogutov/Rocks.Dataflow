using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Commands;
using Rocks.Dataflow.Commands.Tests.FluentTests.Infrastructure;
using Rocks.Dataflow.Fluent;

namespace Rocks.Dataflow.Commands.Tests.FluentTests
{
	[TestClass]
	public class FluentAsyncTests
	{
		[TestInitialize]
		public void TestInitialize ()
		{
			CommandsLibrary.Setup ();
		}


		//[TestMethod]
		//public async Task ActionCommand_CorrectlyProcessed ()
		//{
		//	// arrange
		//	var result = new ConcurrentBag<int> ();

		//	var sut = DataflowCommandsFluent
		//		.ActionCommandAsync<int, IncrementAsyncCommand> (x => new IncrementAsyncCommand { Number = x, Result = result })
		//		.CreateDataflow ();


		//	// act
		//	await sut.Process (new[] { 1, 2, 3 });


		//	// assert
		//	result.Should ().BeEquivalentTo (2, 3, 4);
		//}


		//[TestMethod]
		//public async Task TransformCommand_Action_CorrectlyProcessed ()
		//{
		//	// arrange
		//	var result = new ConcurrentBag<int> ();

		//	var sut = DataflowCommandsFluent
		//		.TransformCommandAsync<char, CharToIntAsyncCommand, int> (c => new CharToIntAsyncCommand { Char = c })
		//		.Action (x => result.Add (x))
		//		.CreateDataflow ();

		//	// act
		//	await sut.Process (new[] { 'a', 'b', 'c' });


		//	// assert
		//	result.Should ().BeEquivalentTo ((int) 'a', (int) 'b', (int) 'c');
		//}



		//[TestMethod]
		//public async Task TransformCommand_ActionCommand_CorrectlyProcessed ()
		//{
		//	// arrange
		//	var result = new ConcurrentBag<int> ();

		//	var sut = DataflowCommandsFluent
		//		.TransformCommandAsync<CharToIntAsyncCommand, IntToCharAsyncCommand> ()
		//		.ActionCommandAsync<CharToIntAsyncCommand, IntToCharAsyncCommand, char> ()
		//		.CreateDataflow ();

		//	var commands = new[] { 'a', 'b', 'c' }
		//		.Select (x => new CharToIntAsyncCommand { Char = x })
		//		.ToList ();


		//	// act
		//	await sut.Process (commands);


		//	// assert
		//	result.Should ().BeEquivalentTo (commands.Select (x => (int) x.Char));
		//}
	}
}