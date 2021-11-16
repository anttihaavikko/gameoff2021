using System.Collections;
using System.Collections.Generic;

namespace Actions
{
    public abstract class BaseAction
    {
        public virtual bool IsPriority => false;
            
        public abstract IEnumerator Activate(Field field);
        public abstract IEnumerable<Card> GetCards();
    }
}