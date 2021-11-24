using System.Collections;

namespace Curses
{
    class NoPreviewCurse : Curse
    {
        public override IEnumerator Apply(Field field)
        {
            yield return null;
        }

        public override string GetTutorial()
        {
            return "You can not (peek) at your (draw pile) at all on this stage.";
        }
    }
}