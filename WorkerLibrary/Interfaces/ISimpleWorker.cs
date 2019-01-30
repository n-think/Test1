using WorkerLibrary.Enums;

namespace WorkerLibrary.Interfaces
{
    public interface ISimpleWorker
    {
        void Start();
        void Stop();
        void Enqueue(params SimpleWork[] workToDo);
        void ResetQueue(SimpleWork workToDo);
        WorkerState State { get; }
    }
}