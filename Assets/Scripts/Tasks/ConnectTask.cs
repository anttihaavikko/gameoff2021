using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;

namespace Tasks
{
    public class ConnectTask : StageTask
    {
        private readonly List<Card> targetCards;
        
        public ConnectTask(Field field, IEnumerable<Vector3> positions)
        {
            targetCards = positions.Select(p => field.CreateAndPlaceCard(p, CardData.Starter())).ToList();
        }

        public override string GetTutorial()
        {
            return "This level has (no par) but you need to (connect) the already placed (cards)!";
        }

        public override string GetText()
        {
            return "Connect cards";
        }

        public override bool Update(Field field)
        {
            if (!targetCards.All(field.WasCardActivated)) return false;
            IsCompleted = true;
            return true;
        }
    }
}