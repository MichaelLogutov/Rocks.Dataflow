using System;
using JetBrains.Annotations;
using Rocks.Commands;


namespace Rocks.Dataflow.Commands.Tests.FluentTests.Infrastructure
{
	public class CharToIntCommand : ICommand<int>
	{
		public char Char { get; set; }
	}


	[UsedImplicitly]
	internal class CharToIntCommandHandler : ICommandHandler<CharToIntCommand, int>
	{
		public int Execute (CharToIntCommand command)
		{
			return command.Char;
		}
	}
}