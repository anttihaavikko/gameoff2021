using System.Collections;

namespace Curses
{
    public abstract class Curse
    {
        public abstract IEnumerator Apply(Field field);
        public abstract string GetTutorial();
    }
}