using System;
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
	public class FluentAsyncTests
	{
		[TestInitialize]
		public void TestInitialize ()
		{
			CommandsLibrary.Setup ();
		}


		[TestMethod]
		public async Task ActionCommand_CorrectlyProcessed ()
		{
			// arrange
			var sut = DataflowCommandsFluent.ActionCommandAsync<IncrementAsyncCommand, Void> ()
			                                .CreateDataflow ();

			var commands = new[] { 1, 2, 3 }.Select (x => new IncrementAsyncCommand { Number = x }).ToList ();


			// act
			await sut.Process (commands);


			// assert
			commands.Select (x => x.Number).Should ().BeEquivalentTo (2, 3, 4);
		}
	}
}