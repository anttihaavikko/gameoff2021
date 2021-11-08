using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Save
{
    public enum Passive
    {
        MultiOnBottom,
        MultiOnTop,
        BiggerHand,
        Options,
        CardPicks,
        LuckyRolls,
        Starchild,
        Bomberman
    }
    
    public static class Passives
    {
        private static readonly Dictionary<Passive, PassiveDetails> ListAll = new Dictionary<Passive, PassiveDetails> {
            {
                Passive.MultiOnBottom,
                new PassiveDetails
                {
                    name = "Bottom",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (card.y <r>==</r> 4)\n\t{\n\t\tstartMultiplier<r>++</r>;\n\t}\n}",
                    repeatable = true
                }
            },
            {
                Passive.MultiOnTop,
                new PassiveDetails
                {
                    name = "Top",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (card.y <r>==</r> 0)\n\t{\n\t\tstartMultiplier<r>++</r>;\n\t}\n}",
                    repeatable = true
                }
            },
            {
                Passive.BiggerHand,
                new PassiveDetails
                {
                    name = "Bigger Hand",
                    description = "<r>void</r> <g>DoSomething</g>() {\n\t<c>// TODO: add implementation</c>\n}",
                    repeatable = true
                }
            },
            {
                Passive.Options,
                new PassiveDetails
                {
                    name = "Options",
                    description = "<r>void</r> <g>DoSomething</g>() {\n\t<c>// TODO: add implementation</c>\n}",
                    repeatable = true
                }
            },
            {
                Passive.CardPicks,
                new PassiveDetails
                {
                    name = "Variety",
                    description = "<r>void</r> <g>DoSomething</g>() {\n\t<c>// TODO: add implementation</c>\n}",
                    repeatable = true
                }
            },
            {
                Passive.LuckyRolls,
                new PassiveDetails
                {
                    name = "Lucky Rolls",
                    description = "<r>bool</c> <g>GetRandomChance</c>(<r>float</c> <o>target</c>)\n{\n\treturn <b>Random</c>.value * <p>0.9f</c> < <o>target</c>;\n}",
                    repeatable = true
                }
            },
            {
                Passive.Starchild,
                new PassiveDetails
                {
                    name = "Starchild",
                    description = "<r>void</c> <g>OnActivate</c>(<b>Pip</c> <b><o>pip</c></c>, <r>int</c> <o>number</c>)\n{\n\t<r>if</c> (<o>number</c> % <p>50</c> <r>==</c> <p>0</c>)\n\t{\n\t\t<b><o>pip</c></c>.<g>TransformToStar</c>();\n\t}\n}",
                    repeatable = false
                }
            },
            {
                Passive.Bomberman,
                new PassiveDetails
                {
                    name = "Bomberman",
                    description = "<r>void</c> <g>OnActivate</c>(<b>Pip</c> <b><o>pip</c></c>, <r>int</c> <o>number</c>)\n{\n\t<r>if</c> (<o>number</c> % <p>20</c> <r>==</c> <p>0</c>)\n\t{\n\t\t<b><o>pip</c></c>.<g>TransformToBomb</c>();\n\t}\n}",
                    repeatable = false
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