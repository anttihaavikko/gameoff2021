namespace Tasks
{
    public abstract class StageTask
    {
        public bool IsCompleted { get; protected set; }

        public abstract string GetTutorial();
        public abstract string GetText();

        public abstract bool Update(Field field);
    }
}