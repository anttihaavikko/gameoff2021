using System.Collections;

namespace Curses
{
    public class DoublerCurse : Curse
    {
        public override IEnumerator Apply(Field field)
        {
            yield return null;
        }

        public override string GetTutorial()
        {
            return "Looks like it's raining (blank cards) here.";
        }
    }
}