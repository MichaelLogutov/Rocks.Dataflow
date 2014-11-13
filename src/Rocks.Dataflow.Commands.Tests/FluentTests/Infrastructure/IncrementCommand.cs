using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Rocks.Commands;
using Void = Rocks.Commands.Void;


namespace Rocks.Dataflow.Commands.Tests.FluentTests.Infrastructure
{
	public class IncrementCommand : ICommand<Void>
	{
		public int Number { get; set; }
		public IProducerConsumerCollection<int> Result { get; set; }
	}


	[UsedImplicitly]
	internal class IncrementCommandHandler : ICommandHandler<IncrementCommand, Void>
	{
		public Void Execute (IncrementCommand command)
		{
			command.Result.TryAdd (command.Number + 1);
			return Void.Result;
		}
	}
}