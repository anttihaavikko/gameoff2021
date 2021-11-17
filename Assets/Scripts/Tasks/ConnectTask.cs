using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;

namespace Tasks
{
    public class ConnectTask : StageTask
    {
        private readonly List<Card> targetCards;

        public ConnectTask(Field field, IEnumerable<CardPlacement> cards)
        {
            targetCards = cards.Select(c => field.CreateAndPlaceCard(c.Position, c.Card)).ToList();
        }
        
        public ConnectTask(Field field, IEnumerable<Vector3> positions)
        {
            targetCards = positions.Select(p => field.CreateAndPlaceCard(p, CardData.Starter())).ToList();
        }

        public override string GetTutorial()
        {
            return "This stage has (no par) but you need to (connect) the already placed (cards)!";
        }

        public override string GetText()
        {
            return "CONNECT CARDS";
        }

        public override bool Update(Field field)
        {
            if (!targetCards.All(field.WasCardActivated)) return false;
            IsCompleted = true;
            return true;
        }
    }

    public class CardPlacement
    {
        public CardData Card { get; }
        public Vector3 Position { get; }
        
        public CardPlacement(CardData card, Vector3 position)
        {
            Card = card;
            Position = position;
        }
    }
}