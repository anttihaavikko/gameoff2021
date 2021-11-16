using UnityEngine;

namespace Tasks
{
    class TouchTask : StageTask
    {
        private readonly string limitAsText;
        private readonly int limit;

        public TouchTask(int limit, string limitAsText)
        {
            this.limit = limit;
            this.limitAsText = limitAsText;
        }
        
        public override string GetTutorial()
        {
            return $"This stage has (no par) but some card you (place) must immediately link to ({limitAsText} pips).";
        }

        public override string GetText()
        {
            return $"LINK TO {limitAsText.ToUpper()}";
        }

        public override bool Update(Field field)
        {
            if (field.PreviousTouched < limit) return false;
            IsCompleted = true;
            return true;
        }
    }
}