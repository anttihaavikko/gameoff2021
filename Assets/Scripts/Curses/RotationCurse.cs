using System.Collections;

namespace Curses
{
    class RotationCurse : Curse
    {
        public override IEnumerator Apply(Field field)
        {
            yield return null;
        }

        public override string GetTutorial()
        {
            return "Everything is somewhat (lopsided) here. Your (cards) might not look the way they've (used to).";
        }
    }
}