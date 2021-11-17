using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Save
{
    public enum Passive
    {
        None,
        MultiOnBottom,
        MultiOnTop,
        BiggerHand,
        Options,
        CardPicks,
        LuckyRolls,
        Starchild,
        Bomberman,
        Detonator,
        MultiOnLeft,
        MultiOnRight,
        StarOnRotate,
        StarOnPush,
        StarOnPull,
        StarTransformer,
        BombTransformer,
        Tactician,
        Chaos,
        StackSize,
        Replacement,
        MegaPush,
        MegaPull,
        Orphanizer,
        DoubleRotations,
        DoublePicks,
        SmallerPars,
        MultiOnCenter,
        PreviewInOrder
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
                    repeatable = true,
                    tutorial = "causes the cards placed on the (bottom edge) to start with (+1) multiplier."
                }
            },
            {
                Passive.MultiOnTop,
                new PassiveDetails
                {
                    name = "Top",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (<o>card</c>.y <r>==</r> <p>0</c>)\n\t{\n\t\tstartMultiplier = <p>1</c> + <p>[LEVEL]</c>;\n\t}\n}",
                    repeatable = true,
                    tutorial = "causes the cards placed on the (top edge) to start with (+1) multiplier."
                }
            },
            {
                Passive.BiggerHand,
                new PassiveDetails
                {
                    name = "Bigger Hand",
                    description = "<r>int</c> <g>GetHandSize</c>()\n{\n\t<r>return</c> DEFAULT_HAND_SIZE + <p>[LEVEL]</c>;\n}",
                    repeatable = true,
                    tutorial = "(increases) your maximum (hand size)."
                }
            },
            {
                Passive.Options,
                new PassiveDetails
                {
                    name = "Options",
                    description = "<r>void</c> <g>PresentCodeInjections</c>()\n{\n\t<r>for</c> (<r>var</c> i = <p>0</c>; i<r> < </c><p>2</c> + <p>[LEVEL]</c>; i<r>++</c>)\n\t{\n\t\t<r>var</c> p = <r>new</c> <b>Passive</c>();\n\t\tp.<g>Present</c>();\n\t}\n}",
                    repeatable = true,
                    tutorial = "makes you get (more options) for future (code injections)."
                }
            },
            {
                Passive.CardPicks,
                new PassiveDetails
                {
                    name = "Variety",
                    description = "<r>void</c> <g>PresentNewCards</c>()\n{\n\t<r>for</c> (<r>var</c> i = <p>0</c>; i<r> < </c><p>3</c> + <p>[LEVEL]</c>; i<r>++</c>)\n\t{\n\t\t<r>var</c> c = <r>new</c> <b>Card</c>();\n\t\tc.<g>Present</c>();\n\t}\n}",
                    repeatable = true,
                    tutorial = "makes you get (more options) on future (cards selections)."
                }
            },
            {
                Passive.LuckyRolls,
                new PassiveDetails
                {
                    name = "Lucky Rolls",
                    description = "<r>bool</c> <g>GetRandomChance</c>(<r>float</c> <o>target</c>)\n{\n\t<r>var</c> mod = <b>Mathf</c>.<g>Pow</c>(<p>0.9f</c>, <p>[LEVEL]</c>);\n\t<r>return</c> <b>Random</c>.value * mod <r><</c> <o>target</c>;\n}",
                    repeatable = true,
                    tutorial = "affects your (luck) and makes you get (better cards)."
                }
            },
            {
                Passive.Starchild,
                new PassiveDetails
                {
                    name = "Starchild",
                    description = "<r>void</c> <g>OnActivate</c>(<b>Pip</c> <b><o>pip</c></c>, <r>int</c> <o>number</c>)\n{\n\t<r>if</c> (<o>number</c> % <p>30</c> <r>==</c> <p>0</c>)\n\t{\n\t\t<b><o>pip</c></c>.<g>TransformToStar</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "makes every (30th) activated (pip) turn to a (star)."
                }
            },
            {
                Passive.Bomberman,
                new PassiveDetails
                {
                    name = "Bomberman",
                    description = "<r>void</c> <g>OnActivate</c>(<b>Pip</c> <b><o>pip</c></c>, <r>int</c> <o>number</c>)\n{\n\t<r>if</c> (<o>number</c> % <p>20</c> <r>==</c> <p>0</c>)\n\t{\n\t\t<b><o>pip</c></c>.<g>TransformToBomb</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "makes every (20th) activated (pip) turn to a (bomb)."
                }
            },
            {
                Passive.Detonator,
                new PassiveDetails
                {
                    name = "Detonator",
                    description = "<r>void</c> <g>OnExplode</c>(<b>Card</c> <o>card</c>)\n{\n\t<o>card</c>.<g>GetNeighbours</c>().<g>Activate</c>();\n}",
                    repeatable = false,
                    tutorial = "causes (exploding cards) to (activate) its (neighbour) cards."
                }
            },
            {
                Passive.MultiOnLeft,
                new PassiveDetails
                {
                    name = "Left",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (<o>card</c>.x <r>==</r> <p>0</c>)\n\t{\n\t\tstartMultiplier = <p>1</c> + <p>[LEVEL]</c>;\n\t}\n}",
                    repeatable = true,
                    tutorial = "causes the cards placed on the (left edge) to start with (+1) multiplier."
                }
            },
            {
                Passive.MultiOnRight,
                new PassiveDetails
                {
                    name = "Right",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (<o>card</c>.x <r>==</r> <p>4</c>)\n\t{\n\t\tstartMultiplier = <p>1</c> + <p>[LEVEL]</c>;\n\t}\n}",
                    repeatable = true,
                    tutorial = "causes the cards placed on the (right edge) to start with (+1) multiplier."
                }
            },
            {
                Passive.StarOnRotate,
                new PassiveDetails
                {
                    name = "Roller",
                    description = "<r>void</c> <g>OnRotate</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>var</c> pip = <o>card</c>.<g>GetRandomPip</c>();\n\tpip.<g>TransformToStar</c>();\n}",
                    repeatable = false,
                    tutorial = "enhances your (rotator cards) to also add a new (random star) to the rotated cards."
                }
            },
            {
                Passive.StarOnPush,
                new PassiveDetails
                {
                    name = "Starpusher",
                    description = "<r>void</c> <g>OnPush</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>var</c> pip = <o>card</c>.<g>GetRandomPip</c>();\n\tpip.<g>TransformToStar</c>();\n}",
                    repeatable = false,
                    tutorial = "enhances your (pusher cards) to also add a new (random star) to the pushed cards."
                }
            },
            {
                Passive.StarOnPull,
                new PassiveDetails
                {
                    name = "Starpuller",
                    description = "<r>void</c> <g>OnPull</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>var</c> pip = <o>card</c>.<g>GetRandomPip</c>();\n\tpip.<g>TransformToStar</c>();\n}",
                    repeatable = false,
                    tutorial = "enhances your (puller cards) to also add a new (random star) to the pulled cards."
                }
            },
            {
                Passive.StarTransformer,
                new PassiveDetails
                {
                    name = "Defuse Kit",
                    description = "<r>void</c> <g>OnActivate</c>(<b>Pip</c> <b><o>pip</c></c>)\n{\n\t<r>if</c> (<o>pip</c>.<g>IsBomb</c>())\n\t{\n\t\t<o>pip</c>.<g>TransformToStar</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "saves you from (destruction) by replacing activated (bombs) with (stars)."
                }
            },
            {
                Passive.BombTransformer,
                new PassiveDetails
                {
                    name = "Danger Levels",
                    description = "<r>void</c> <g>OnActivate</c>(<b>Pip</c> <b><o>pip</c></c>)\n{\n\t<r>if</c> (<o>pip</c>.<g>IsStar</c>())\n\t{\n\t\t<o>pip</c>.<g>TransformToBomb</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "wreaks some (havoc) by replacing activated (stars) with (bombs)."
                }
            },
            {
                Passive.Tactician,
                new PassiveDetails
                {
                    name = "Tactician",
                    description = "<r>void</c> <g>OnTurnStart</c>(<r>int</c> <o><o>turn</c></c>)\n{\n\tpostMultiplier = <o><o>turn</c></c> % <p>2</c> ? <p>3</c> : <p>0</c>;\n}",
                    repeatable = false,
                    tutorial = "makes your (even numbered turns) gain (triple) the points but (none) on (odd) ones."
                }
            },
            {
                Passive.Chaos,
                new PassiveDetails
                {
                    name = "Chaos",
                    description = "<r>void</c> <g>OnTurnStart</c>(<r>int</c> <o><o>turn</c></c>)\n{\n\t<r>if</c> (<o><o>turn</c></c> % 5 <r>==</c> 0)\n\t{\n\t\t<g>AddRandomCard</c>();\n\t\t<r>return</c>;\n\t}\n\n\t<g>DrawCard</c>();\n}",
                    repeatable = false,
                    tutorial = "gives you a (random card) instead of (drawing) one every (5 turns)."
                }
            },
            {
                Passive.StackSize,
                new PassiveDetails
                {
                    name = "Stack Size",
                    description = "<r>void</c> <g>OnProcess</c>(<b><g>Act</c>ionQueue</c> <o>queue</c>)\n{\n\tif (<o>queue</c>.Count <r>>=</c> <p>40</c> + <p>40</c> * <p>[LEVEL]</c>)\n\t{\n\t\t<g>Tilt</c>();\n\t\t<r>return</c>;\n\t}\n\n\t<o>queue</c>.<g>Pop</c>().<g>Act</c>();\n}",
                    repeatable = true,
                    tutorial = "increases the (maximum action stack size) giving you more (combo) chances."
                }
            },
            {
                Passive.Replacement,
                new PassiveDetails
                {
                    name = "Replacement",
                    description = "<r>void</c> <g>OnDestroy</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>var</c> next = <r>new</c> <g>Card</c>();\n\t<g>PlaceOn</c>(next, <o>card</c>.<g>GetPosition</c>());\n}",
                    repeatable = false,
                    tutorial = "causes (exploding cards) to be replaced with a new (random card)."
                }
            },
            {
                Passive.MegaPush,
                new PassiveDetails
                {
                    name = "Force",
                    description = "<r>void</c> <g>OnPlace</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>if</c> (<o>card</c>.<g>IsPusher</c>())\n\t{\n\t\t<o>card</c>.<g>PushAll</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "enhances your (push cards) to affect (every) other card in marked directions and also (extends) its range."
                }
            },
            {
                Passive.MegaPull,
                new PassiveDetails
                {
                    name = "Vacuum",
                    description = "<r>void</c> <g>OnPlace</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>if</c> (<o>card</c>.<g>IsPuller</c>())\n\t{\n\t\t<o>card</c>.<g>PullAll</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "enhances your (pull cards) to affect (every) other card in marked directions and also (extends) its range."
                }
            },
            {
                Passive.Orphanizer,
                new PassiveDetails
                {
                    name = "Orphanizer",
                    description = "<r>void</c> <g>OnPlace</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>var</c> amt = <o>card</c>.<g>GetNeighbourCount</c>();\n\t<r>if</c> (<g>NotFirstTurn</c>() <r>&&</c> amt <r>==</c> <p>0</c>)\n\t{\n\t\t<o>card</c>.<g>Explode</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "allows you to (discard) bad cards by placing them on (their own) so they'll get (destroyed)."
                }
            },
            {
                Passive.DoubleRotations,
                new PassiveDetails
                {
                    name = "One Eighty",
                    description = "<r>void</c> <g>OnPlace</c>(<b>Card</c> <o>card</c>)\n{\n\t<r>if</c> (<o>card</c>.<g>IsRotator</c>())\n\t{\n\t\t<o>card</c>.<g>RotateNeighbours</c>();\n\t\t<o>card</c>.<g>RotateNeighbours</c>();\n\t}\n}",
                    repeatable = false,
                    tutorial = "makes your (rotator cards) trigger twice."
                }
            },
            {
                Passive.DoublePicks,
                new PassiveDetails
                {
                    name = "Greed",
                    description = "<r>int</c> <g>GetCardPickCount</c>()\n{\n\t<r>return</c> DEFAULT_PICK_COUNT + <p>1</c>;\n}",
                    repeatable = false,
                    tutorial = "allows you to pick (two cards) after each stage."
                }
            },
            {
                Passive.SmallerPars,
                new PassiveDetails
                {
                    name = "Underachiever",
                    description = "<r>override</c> <r>int</c> <g>GetStagePar</c>()\n{\n\t<r>var</c> mod = <b>Mathf</c>.<g>Pow</c>(<p>0.80f</c>, <p>[LEVEL]</c>);\n\t<r>return</c> <o>base</c>.<g>GetStagePar</c>() * mod;\n}",
                    repeatable = true,
                    tutorial = "makes the (pars) of stages more easily (reachable)."
                }
            },
            {
                Passive.MultiOnCenter,
                new PassiveDetails
                {
                    name = "Center",
                    description = "<r>void</r> <g>OnPlace</g>(<b>Card</b> <o>card</o>)\n{\n\t<r>if</r> (<o>card</c>.x <r>==</r> <p>2</c> <r>&&</c> <o>card</c>.y <r>==</r> <p>2</c>)\n\t{\n\t\tstartMultiplier = <p>1</c> + <p>[LEVEL]</c> * <p>5</c>;\n\t}\n}",
                    repeatable = true,
                    tutorial = "causes the cards placed on the (center cell) to start with (+5) multiplier."
                }
            },
            {
                Passive.PreviewInOrder,
                new PassiveDetails
                {
                    name = "Visions",
                    description = "<b>IList</c><<b>Card</c>> <g>DeckContentsForPreview</c>()\n{\n\t<r>return</c> deck.<g>OrderBy</c>(<o>c</c> => <o>c</c>.Order);\n}",
                    repeatable = false,
                    tutorial = "allows you to see the (draw pile) cards in correct (order)."
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

        public static IEnumerable<Passive> GetAll()
        {
            return ListAll.Select(pair => pair.Key);
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
        public string tutorial;
    }
}