using System.Collections;

namespace Actions
{
    public class ActivateAction : BaseAction
    {
        private readonly Card card;
        private readonly int multiplier;
        
        public ActivateAction(Card card, int multiplier)
        {
            this.card = card;
            this.multiplier = multiplier;
        }
        
        public override IEnumerator Activate(Field field)
        {
            field.Activate(card, multiplier);
            yield break;
        }
    }
}