using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class ActionQueue
    {
        private readonly List<BaseAction> queue;

        public ActionQueue()
        {
            queue = new List<BaseAction>();
        }

        public IEnumerator Process(Field field)
        {
            while (queue.Any())
            {
                var action = queue[0];
                queue.Remove(action);
                yield return action.Activate(field);
            }
        }

        public void Add(BaseAction action)
        {
            queue.Add(action);
        }
    }

    public abstract class BaseAction
    {
        public abstract IEnumerator Activate(Field field);
    }

    public class ScoreAction : BaseAction
    {
        private readonly Card card;
        private readonly int multiplier;
        
        public ScoreAction(Card card, int multiplier)
        {
            this.card = card;
            this.multiplier = multiplier;
        }
        
        public override IEnumerator Activate(Field field)
        {
            yield return field.ScoreCard(card, multiplier);
        }
    }

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
    
    public class RotateAction : BaseAction
    {
        private readonly List<Card> cards;
        private readonly int multiplier;
        private readonly bool clockwise;
        
        public RotateAction(IEnumerable<Card> cards, int multiplier, bool clockwise)
        {
            this.cards = cards.ToList();
            this.multiplier = multiplier;
            this.clockwise = clockwise;
        }
        
        public override IEnumerator Activate(Field field)
        {
            cards.ForEach(c => field.Rotate(c, clockwise));
            yield return new WaitForSeconds(0.35f);
        }
    }
    
    public class DestroyAction : BaseAction
    {
        private readonly List<Card> cards;
        
        public DestroyAction(Card card)
        {
            cards = new List<Card>{ card };
        }

        public DestroyAction(IEnumerable<Card> cards)
        {
            this.cards = cards.ToList();
        }
        
        public override IEnumerator Activate(Field field)
        {
            cards.ForEach(field.RemoveCard);
            yield return new WaitForSeconds(0.5f);
        }
    }
}