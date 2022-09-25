namespace Focus.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    static class Extensions
    {
        public static bool IsEqual<T>(this IEnumerable<T> expected, IEnumerable<T> actual)
        {
            return !actual.Any(x => !expected.Any(y => x.Equals(y))) || !expected.Any(x => !actual.Any(y => x.Equals(y)));
        }
    }
}
