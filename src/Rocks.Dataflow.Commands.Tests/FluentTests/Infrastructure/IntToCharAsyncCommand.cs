using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rocks.Commands;


namespace Rocks.Dataflow.Commands.Tests.FluentTests.Infrastructure
{
	public class IntToCharAsyncCommand : IAsyncCommand<char>
	{
		public int Int { get; set; }
	}


	[UsedImplicitly]
	internal class IntToCharAsyncCommandCommandHandler : IAsyncCommandHandler<IntToCharAsyncCommand, char>
	{
		public async Task<char> ExecuteAsync (IntToCharAsyncCommand command, CancellationToken cancellationToken = new CancellationToken ())
		{
			await Task.Yield ();
			return (char) command.Int;
		}
	}
}