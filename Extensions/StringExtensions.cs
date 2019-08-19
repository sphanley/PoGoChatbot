using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoGoChatbot.Extensions
{
    public static class StringExtensions
    {
        public static string CommaSeparateWithAnd(this IEnumerable<string> array, bool quoted = false)
        {
            var strings = quoted ? array.Select(str => $"\"{str}\"") : array;

            var sb = new StringBuilder("");
            if (strings == null) return null;

            if (!strings.Any()) return sb.ToString();
            if (strings.Count() == 1) return strings.First();

            sb.Append(string.Join(", ", strings.Take(array.Count() - 1)));

            sb.Append(" and ");
            sb.Append(strings.LastOrDefault());

            return sb.ToString();
        }
    }
}
