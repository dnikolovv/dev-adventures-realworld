using Conduit.Core;
using System.Collections.Generic;

namespace Conduit.Business.Extensions
{
    public static class StringExtensions
    {
        public static Error ToError(this IEnumerable<string> errors) =>
            new Error(errors);
    }
}
