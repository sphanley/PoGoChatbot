using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoGoChatbot.Extensions
{
    public static class StringExtensions
    {
        public static string CommaSeparateWithAnd(this IEnumerable<string> array)
        {
            var sb = new StringBuilder("");
            if (array == null) return null;

            if (!array.Any()) return sb.ToString();
            if (array.Count() == 1) return array.First();

            sb.Append(string.Join(", ", array.Take(array.Count() - 1)));

            sb.Append(" and ");
            sb.Append(array.LastOrDefault());

            return sb.ToString();
        }
    }
}
