using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Commands;
using Rocks.Dataflow.Fluent.Builders;
using Void = Rocks.Commands.Void;

namespace Rocks.Dataflow.Commands.Fluent
{
	//public static partial class DataflowCommandsFluent
	//{
	//	/// <summary>
	//	///     Creates the dataflow from the single <see cref="ActionBlock{TCommand}" />
	//	///     using <paramref name="createCommand" /> function to create instance of
	//	///     <see cref="ICommand{Void}" /> that will be used as the block body.
	//	/// </summary>
	//	public static DataflowActionBuilder<TInput, TInput> ActionCommand<TInput, TCommand> (
	//		[NotNull] Func<TInput, TCommand> createCommand)
	//		where TCommand : ICommand<Void>
	//	{
	//		if (createCommand == null)
	//			throw new ArgumentNullException ("createCommand");

	//		return new DataflowActionBuilder<TInput, TInput>
	//			(null,
	//			 x =>
	//			 {
	//				 var command = createCommand (x);

	//				 CommandsLibrary.CommandsProcessor.Execute (command);
	//			 });
	//	}


	//	/// <summary>
	//	///     Creates the dataflow from the single <see cref="ActionBlock{TCommand}" />
	//	///     using <paramref name="createCommand" /> function to create instance of
	//	///     <see cref="IAsyncCommand{Void}" /> that will be used as the block body.
	//	/// </summary>
	//	public static DataflowActionBuilder<TInput, TInput> ActionCommandAsync<TInput, TCommand> (
	//		[NotNull] Func<TInput, TCommand> createCommand,
	//		CancellationToken cancelationToken = default (CancellationToken))
	//		where TCommand : IAsyncCommand<Void>
	//	{
	//		if (createCommand == null)
	//			throw new ArgumentNullException ("createCommand");

	//		return new DataflowActionBuilder<TInput, TInput>
	//			(null,
	//			 async x =>
	//			 {
	//				 var command = createCommand (x);

	//				 await CommandsLibrary.CommandsProcessor
	//									  .ExecuteAsync (command, cancelationToken)
	//									  .ConfigureAwait (false);
	//			 });
	//	}


	//	///// <summary>
	//	/////     Ends the dataflow with the <see cref="ActionBlock{TInput}" /> using using <see cref="ICommand{TCommandResult}"/> as a body.
	//	///// </summary>
	//	//public static DataflowActionBuilder<TStart, TCommand> ActionCommand<TStart, TCommand, TCommandResult> (
	//	//	[NotNull] this IDataflowBuilder<TStart, TCommand> previousBuilder)
	//	//	where TCommand : ICommand<TCommandResult>
	//	//{
	//	//	if (previousBuilder == null)
	//	//		throw new ArgumentNullException ("previousBuilder");

	//	//	return new DataflowActionBuilder<TStart, TCommand> (previousBuilder, command => CommandsLibrary.CommandsProcessor.Execute (command));
	//	//}


	//	///// <summary>
	//	/////     Ends the dataflow with the <see cref="ActionBlock{TInput}" /> using <see cref="IAsyncCommand{TCommandResult}"/> as a body.
	//	///// </summary>
	//	//public static DataflowActionBuilder<TStart, TCommand> ActionCommandAsync<TStart, TCommand, TCommandResult> (
	//	//	[NotNull] this IDataflowBuilder<TStart, TCommand> previousBuilder)
	//	//	where TCommand : IAsyncCommand<TCommandResult>
	//	//{
	//	//	if (previousBuilder == null)
	//	//		throw new ArgumentNullException ("previousBuilder");

	//	//	return new DataflowActionBuilder<TStart, TCommand> (previousBuilder, command => CommandsLibrary.CommandsProcessor.ExecuteAsync (command));
	//	//}
	//}
}