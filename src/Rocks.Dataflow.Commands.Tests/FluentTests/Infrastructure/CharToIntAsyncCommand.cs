using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rocks.Commands;


namespace Rocks.Dataflow.Commands.Tests.FluentTests.Infrastructure
{
	public class CharToIntAsyncCommand : IAsyncCommand<int>
	{
		public char Char { get; set; }
	}


	[UsedImplicitly]
	internal class CharToIntAsyncCommandHandler : IAsyncCommandHandler<CharToIntAsyncCommand, int>
	{
		public async Task<int> ExecuteAsync (CharToIntAsyncCommand command, CancellationToken cancellationToken = new CancellationToken ())
		{
			await Task.Yield ();
			return command.Char;
		}
	}
}