using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocks.Commands;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Commands.Fluent;
using Rocks.Dataflow.Commands.Tests.FluentTests.Infrastructure;
using Void = Rocks.Commands.Void;

namespace Rocks.Dataflow.Commands.Tests.FluentTests
{
	[TestClass]
	public class FluentSyncTests
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
		//	var sut = DataflowCommandsFluent
		//		.ActionCommand<IncrementCommand, Void> ()
		//		.CreateDataflow ();

		//	var commands = new[] { 1, 2, 3 }
		//		.Select (x => new IncrementCommand { Number = x })
		//		.ToList ();


		//	// act
		//	await sut.Process (commands);


		//	// assert
		//	commands.Select (x => x.Number).Should ().BeEquivalentTo (2, 3, 4);
		//}


		//[TestMethod]
		//public async Task TransformCommand_Action_CorrectlyProcessed ()
		//{
		//	// arrange
		//	var result = new ConcurrentBag<int> ();

		//	var sut = DataflowCommandsFluent
		//		.TransformCommand<CharToIntCommand, IntToCharCommand> ()
		//		.Action (x => result.Add (x.Int))
		//		.CreateDataflow ();

		//	var commands = new[] { 'a', 'b', 'c' }
		//		.Select (x => new CharToIntCommand { Char = x })
		//		.ToList ();


		//	// act
		//	await sut.Process (commands);


		//	// assert
		//	result.Should ().BeEquivalentTo (commands.Select (x => (int) x.Char));
		//}
	}
}