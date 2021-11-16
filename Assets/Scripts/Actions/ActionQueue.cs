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
                var action = GetNextAction();
                queue.Remove(action);
                yield return action.Activate(field);
                actions++;

                if (actions >= field.GetStackSize())
                {
                    field.Tilt();
                    yield break;
                }
            }
        }

        private BaseAction GetNextAction()
        {
            return queue.FirstOrDefault(a => a.IsPriority) ?? queue[0];
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

        public bool Any()
        {
            return queue.Count > 0;
        }
    }
}