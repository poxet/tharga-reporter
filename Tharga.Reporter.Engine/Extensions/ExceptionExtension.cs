using System;

namespace Tharga.Reporter.Engine
{
    internal static class ExceptionExtension
    {
        public static Exception AddData(this Exception exp, object key, object value)
        {
            exp.Data.Add(key, value);
            return exp;
        }
    }
}