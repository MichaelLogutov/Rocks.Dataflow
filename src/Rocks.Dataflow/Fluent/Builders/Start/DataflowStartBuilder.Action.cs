using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Action;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
	public partial class DataflowStartBuilder<TStart>
	{
		/// <summary>
		///     Ends the dataflow from the <see cref="ActionBlock{TStart}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowActionBuilder<TStart, TStart> ActionAsync (Func<TStart, Task> process)
		{
			return new DataflowActionBuilder<TStart, TStart> (null, process);
		}


		/// <summary>
		///     Ends the dataflow from the <see cref="ActionBlock{TStart}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowActionBuilder<TStart, TStart> Action (Action<TStart> process)
		{
			return new DataflowActionBuilder<TStart, TStart> (null, process);
		}
	}
}