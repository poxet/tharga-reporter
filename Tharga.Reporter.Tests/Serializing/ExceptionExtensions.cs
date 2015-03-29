using System;

namespace Tharga.Reporter.Tests.Serializing
{
    public static class ExceptionExtensions
    {
        public static Type ToType(this Exception item)
        {
            return item.GetType();
        }
    }
}