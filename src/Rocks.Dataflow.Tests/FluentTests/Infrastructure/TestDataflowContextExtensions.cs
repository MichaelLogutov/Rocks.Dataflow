using System.Collections.Generic;
using System.Linq;

namespace Rocks.Dataflow.Tests.FluentTests.Infrastructure
{
	internal static class TestDataflowContextExtensions
	{
		public static IList<TestDataflowContext<T>> CreateDataflowContexts<T> (this IEnumerable<T> items)
		{
			return items.Select (item => new TestDataflowContext<T> { Data = item }).ToList ();
		}
	}
}