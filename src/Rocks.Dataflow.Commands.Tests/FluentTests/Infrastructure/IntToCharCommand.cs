using System;
using JetBrains.Annotations;
using Rocks.Commands;


namespace Rocks.Dataflow.Commands.Tests.FluentTests.Infrastructure
{
	public class IntToCharCommand : ICommand<char>
	{
		public int Int { get; set; }
	}


	[UsedImplicitly]
	internal class IntToCharCommandHandler : ICommandHandler<IntToCharCommand, char>
	{
		public char Execute (IntToCharCommand command)
		{
			return (char) command.Int;
		}
	}
}