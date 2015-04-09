using System.Collections.Generic;

namespace Tharga.Reporter.SampleConsole.Commands.ExampleCommands
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> TakeAllButFirst<T>(this IEnumerable<T> items)
        {
            var index = 0;
            foreach (var item in items)
            {
                if (index > 0)
                    yield return item;
                index++;
            }
        }
    }
}