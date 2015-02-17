using System;
using System.Linq;

namespace Rocks.Dataflow.Extensions
{
    internal static class Helpers
    {
        public static bool Implements (this Type inspectedType, Type desiredType)
        {
            if (!desiredType.IsGenericTypeDefinition)
                return inspectedType.GetInterfaces ().Any (desiredType.IsAssignableFrom);

            return inspectedType.ImplementingOpenGeneric (desiredType);
        }


        private static bool ImplementingOpenGeneric (this Type examinedType, Type openGenericType)
        {
            if (openGenericType.IsInterface)
                return examinedType.GetInterfaces ().Any (x => x.IsGenericType && x.GetGenericTypeDefinition () == openGenericType);

            if (examinedType.IsGenericType && examinedType.GetGenericTypeDefinition () == openGenericType)
                return true;

            return examinedType.BaseType != null && ImplementingOpenGeneric (examinedType.BaseType, openGenericType);
        }
    }
}