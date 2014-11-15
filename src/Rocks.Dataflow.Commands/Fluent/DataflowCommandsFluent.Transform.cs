using System;
using System.Threading.Tasks.Dataflow;
using JetBrains.Annotations;
using Rocks.Commands;
using Rocks.Dataflow.Fluent.Builders;

namespace Rocks.Dataflow.Commands.Fluent
{
	//public static partial class DataflowCommandsFluent
	//{
	//	/// <summary>
	//	///     Starts the dataflow with the <see cref="TransformBlock{TInput, TOutput}" />
	//	///     using <paramref name="createCommand" /> function to create instance of
	//	///     <see cref="ICommand{TOutput}" /> that will be used as the block body.
	//	/// </summary>
	//	public static DataflowTranformBuilder<TInput, TInput, TCommandResult> TransformCommand<TInput, TCommand, TCommandResult> (
	//		[NotNull] Func<TInput, TCommand> createCommand)
	//		where TCommand : ICommand<TCommandResult>
	//	{
	//		if (createCommand == null)
	//			throw new ArgumentNullException ("createCommand");

	//		return new DataflowTranformBuilder<TInput, TInput, TCommandResult>
	//			(null,
	//			 x =>
	//			 {
	//				 var command = createCommand (x);

	//				 var result = CommandsLibrary.CommandsProcessor.Execute (command);

	//				 return result;
	//			 });
	//	}


	//	/// <summary>
	//	///     Starts the dataflow with the <see cref="TransformBlock{TInput, TOutput}" />
	//	///     using <paramref name="createCommand" /> function to create instance of
	//	///     <see cref="IAsyncCommand{TOutput}" /> that will be used as the block body.
	//	/// </summary>
	//	public static DataflowTranformBuilder<TInput, TInput, TCommandResult> TransformCommandAsync<TInput, TCommand, TCommandResult> (
	//		[NotNull] Func<TInput, TCommand> createCommand)
	//		where TCommand : IAsyncCommand<TCommandResult>
	//	{
	//		if (createCommand == null)
	//			throw new ArgumentNullException ("createCommand");

	//		return new DataflowTranformBuilder<TInput, TInput, TCommandResult>
	//			(null,
	//			 async x =>
	//			 {
	//				 var command = createCommand (x);

	//				 var result = await CommandsLibrary.CommandsProcessor
	//												   .ExecuteAsync (command)
	//												   .ConfigureAwait (false);

	//				 return result;
	//			 });
	//	}
	//}
}