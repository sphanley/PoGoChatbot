using System;
using System.Collections.Generic;
using System.Linq;

namespace PoGoChatbot.Models
{
    [Serializable]
    public class TypeMatchupList : Dictionary<string, MatchupsForType> { }

    [Serializable]
    public class MatchupsForType : Dictionary<string, decimal>
    {
        public static MatchupsForType operator *(MatchupsForType a, MatchupsForType b)
        {
            if (!a.Keys.SequenceEqual(b.Keys)) throw new ArgumentException();

            var product = new MatchupsForType();
            foreach (var key in a.Keys)
            {
                product[key] = a[key] * b[key];
            }
            return product;
        }
    }
}
