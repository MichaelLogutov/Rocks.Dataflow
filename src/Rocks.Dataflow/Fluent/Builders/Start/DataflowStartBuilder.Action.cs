using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Action;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
	public partial class DataflowStartBuilder<TStart> : IDataflowStartBuilder<TStart>
	{
		/// <summary>
		///     Ends the dataflow from the <see cref="ActionBlock{TStart}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowActionBuilder<TStart, TStart> DoAsync (Func<TStart, Task> process)
		{
			return new DataflowActionBuilder<TStart, TStart> (null, process);
		}


		/// <summary>
		///     Ends the dataflow from the <see cref="ActionBlock{TStart}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowActionBuilder<TStart, TStart> Do (Action<TStart> process)
		{
			return new DataflowActionBuilder<TStart, TStart> (null, process);
		}
	}
}