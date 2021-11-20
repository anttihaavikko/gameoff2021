using UnityEngine;

namespace Tasks
{
    class TimedTask : StageTask
    {
        private int seconds;

        public TimedTask(int level, int perCard)
        {
            seconds = Mathf.Min(level + 5, 25) * perCard;
        }
        
        public override string GetTutorial()
        {
            return "This stage has (no par) but you must (finish) it in the allotted (time).";
        }

        public override string GetText()
        {
            return $"TIME {seconds}";
        }

        public override bool Update(Field field)
        {
            return false;
        }

        public bool HasTime()
        {
            return seconds > 0;
        }

        public void TickDown()
        {
            if (seconds > 0)
            {
                seconds--;   
            }
        }
        
        public void MarkDone()
        {
            IsCompleted = true;
        }

        public string GetTimeLeft()
        {
            return seconds == 1 ? "a second" : $"{seconds} seconds";
        }
    }
}