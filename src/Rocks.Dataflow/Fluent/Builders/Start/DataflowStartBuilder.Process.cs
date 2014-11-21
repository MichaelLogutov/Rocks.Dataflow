using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Rocks.Dataflow.Fluent.Builders.Tranform;

namespace Rocks.Dataflow.Fluent.Builders.Start
{
	public partial class DataflowStartBuilder<TStart>
	{
		/// <summary>
		///     Continues the dataflow from <see cref="TransformBlock{TStart,TStart}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowProcessBuilder<TStart, TStart> ProcessAsync (Func<TStart, Task> process)
		{
			return new DataflowProcessBuilder<TStart, TStart> (null, process);
		}


		/// <summary>
		///     Continues the dataflow from <see cref="TransformBlock{TStart,TStart}" /> using <paramref name="process"/> as a body.
		/// </summary>
		public DataflowProcessBuilder<TStart, TStart> Process (Action<TStart> process)
		{
			return new DataflowProcessBuilder<TStart, TStart> (null, process);
		}
	}
}