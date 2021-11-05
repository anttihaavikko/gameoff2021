using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
}