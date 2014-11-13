using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Commands;
using Rocks.Dataflow.Fluent;
using Rocks.Dataflow.Fluent.Builders;

namespace Rocks.Dataflow.Commands.Fluent
{
	public static partial class DataflowCommandsFluent
	{
		/// <summary>
		///     Creates the dataflow from the single <see cref="ActionBlock{TCommand}" /> using <see cref="ICommand{TCommandResult}"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TCommand, TCommand> ActionCommand<TCommand, TCommandResult> ()
			where TCommand : ICommand<TCommandResult>
		{
			return new DataflowActionBuilder<TCommand, TCommand> (null, command => CommandsLibrary.CommandsProcessor.Execute (command));
		}


		/// <summary>
		///     Creates the dataflow from the single <see cref="ActionBlock{TCommand}" /> using <see cref="IAsyncCommand{TCommandResult}"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TCommand, TCommand> ActionCommandAsync<TCommand, TCommandResult> ()
			where TCommand : IAsyncCommand<TCommandResult>
		{
			return new DataflowActionBuilder<TCommand, TCommand> (null, command => CommandsLibrary.CommandsProcessor.ExecuteAsync (command));
		}


		/// <summary>
		///     Ends the dataflow with the <see cref="ActionBlock{TInput}" /> using using <see cref="ICommand{TCommandResult}"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TStart, TCommand> ActionCommand<TStart, TCommand, TCommandResult> (
			[NotNull] this IDataflowBuilder<TStart, TCommand> previousBuilder)
			where TCommand : ICommand<TCommandResult>
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowActionBuilder<TStart, TCommand> (previousBuilder, command => CommandsLibrary.CommandsProcessor.Execute (command));
		}


		/// <summary>
		///     Ends the dataflow with the <see cref="ActionBlock{TInput}" /> using <see cref="IAsyncCommand{TCommandResult}"/> as a body.
		/// </summary>
		public static DataflowActionBuilder<TStart, TCommand> ActionCommandAsync<TStart, TCommand, TCommandResult> (
			[NotNull] this IDataflowBuilder<TStart, TCommand> previousBuilder)
			where TCommand : IAsyncCommand<TCommandResult>
		{
			if (previousBuilder == null)
				throw new ArgumentNullException ("previousBuilder");

			return new DataflowActionBuilder<TStart, TCommand> (previousBuilder, command => CommandsLibrary.CommandsProcessor.ExecuteAsync (command));
		}
	}
}