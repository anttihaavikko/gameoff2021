using System.Collections;
using Save;

namespace Curses
{
    public class DoublerCurse : Curse
    {
        private readonly string cardName;

        public CardData CardType { get; }

        public DoublerCurse(CardData cardType, string cardName)
        {
            CardType = cardType;
            this.cardName = cardName;
        }
        
        public override IEnumerator Apply(Field field)
        {
            yield return null;
        }

        public override string GetTutorial()
        {
            return $"Looks like it's raining ({cardName}) here.";
        }
    }
}