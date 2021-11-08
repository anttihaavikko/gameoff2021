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
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (<o>card</c>.y <r>==</r> <p>4</c>)\n\t{\n\t\tstartMultiplier = <p>1</c> + <p>[LEVEL]</c>;\n\t}\n}",
                    repeatable = true
                }
            },
            {
                Passive.MultiOnTop,
                new PassiveDetails
                {
                    name = "Top",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (<o>card</c>.y <r>==</r> <p>0</c>)\n\t{\n\t\tstartMultiplier = <p>1</c> + <p>[LEVEL]</c>;\n\t}\n}",
                    repeatable = true
                }
            },
            {
                Passive.BiggerHand,
                new PassiveDetails
                {
                    name = "Bigger Hand",
                    description = "<r>int</c> <g>GetHandSize</c>()\n{\n\t<r>return</c> DEFAULT_HAND_SIZE + <p>[LEVEL]</c>;\n}",
                    repeatable = true
                }
            },
            {
                Passive.Options,
                new PassiveDetails
                {
                    name = "Options",
                    description = "<r>void</c> <g>PresentCodeInjections</c>()\n{\n\t<r>for</c> (<r>var</c> i = 0; i<r> < </c><p>2</c> + <p>[LEVEL]</c>; i<r>++</c>)\n\t{\n\t\t<r>var</c> p = <r>new</c> <b>Passive</c>();\n\t\tp.<g><r>Present</c></c>();\n\t}\n}",
                    repeatable = true
                }
            },
            {
                Passive.CardPicks,
                new PassiveDetails
                {
                    name = "Variety",
                    description = "<r>void</c> <g>PresentNewCards</c>()\n{\n\t<r>for</c> (<r>var</c> i = 0; i<r> < </c><p>3</c> + <p>[LEVEL]</c>; i<r>++</c>)\n\t{\n\t\t<r>var</c> c = <r>new</c> <b>Card</c>();\n\t\tc.<g><r>Present</c></c>();\n\t}\n}",
                    repeatable = true
                }
            },
            {
                Passive.LuckyRolls,
                new PassiveDetails
                {
                    name = "Lucky Rolls",
                    description = "<r>bool</c> <g>GetRandomChance</c>(<r>float</c> <o>target</c>)\n{\n\t<r>var</c> mod = <b>Mathf</c>.<g>Pow</c>(<p>0.9f</c>, <p>[LEVEL]</c>);\n\t<r>return</c> <b>Random</c>.value * mod <p><</c> <o>target</c>;\n}",
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