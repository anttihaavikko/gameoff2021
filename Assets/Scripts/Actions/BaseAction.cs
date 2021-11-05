using System.Collections;

namespace Actions
{
    public abstract class BaseAction
    {
        public abstract IEnumerator Activate(Field field);
    }
}