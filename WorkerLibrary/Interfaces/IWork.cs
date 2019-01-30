namespace WorkerLibrary.Interfaces
{
    public interface IWork
    {
        bool Completed { get; }
        string Name { get; }
        void DoWork();
    }
}