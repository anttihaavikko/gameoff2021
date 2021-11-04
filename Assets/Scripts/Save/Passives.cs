using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Save
{
    public enum Passive
    {
        MultiOnBottom,
        MultiOnTop
    }
    
    public static class Passives
    {
        private static readonly Dictionary<Passive, PassiveDetails> ListAll = new Dictionary<Passive, PassiveDetails> {
            {
                Passive.MultiOnBottom,
                new PassiveDetails
                {
                    name = "Bottom",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Component</b> <o>component</o>)\n{\n\t<r>if</r> (component.y <r>==</r> 4)\n\t{\n\t\tstartMultiplier<r>++</r>;\n\t}\n}",
                    repeatable = true
                }
            },
            {
                Passive.MultiOnTop,
                new PassiveDetails
                {
                    name = "Top",
                    description = "<r>void</r> <g>DoSomething</g>() {\n\t<c>// TODO: add implementation</c>\n}",
                    repeatable = true
                }
            }
        };

        public static IEnumerable<Passive> GetRandom(List<Passive> exclude, int amount = 1)
        {
            return ListAll
                .Where(p => !exclude.Contains(p.Key) || p.Value.repeatable)
                .OrderBy(_ => Random.value)
                .Take(amount)
                .Select(pair => pair.Key);
        }

        public static PassiveDetails GetDetails(Passive passive)
        {
            return ListAll[passive];
        }
    }

    public class PassiveDetails
    {
        public string name;
        public string description;
        public bool repeatable;
    }
}