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
            var actions = 0;
            
            while (queue.Any())
            {
                var action = queue[0];
                queue.Remove(action);
                yield return action.Activate(field);
                actions++;

                if (actions > 20)
                {
                    field.Tilt();
                    yield break;
                }
            }
        }

        public void Add(BaseAction action)
        {
            queue.Add(action);
        }

        public void Clear()
        {
            var cards = queue.SelectMany(a => a.GetCards()).Distinct().ToList();
            cards.ForEach(c => c.SetBorderColorTo(Color.black));
            queue.Clear();
        }
    }
}