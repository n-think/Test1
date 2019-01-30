using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WorkerLibrary.Enums;
using WorkerLibrary.Interfaces;

namespace WorkerLibrary
{
    public class Worker : ISimpleWorker
    {
        private Worker()
        {
        }

        #region properties & fields

        public static Worker GetInstance { get; } = new Worker();

        private readonly ConcurrentQueue<SimpleWork> _internalQueue = new ConcurrentQueue<SimpleWork>();

        public WorkerState State { get; private set; } = WorkerState.Stopped;

        private readonly object _stateLock = new object();

        #endregion


        #region events

        public event EventHandler<WorkerEventArgs> WorkStarting;
        public event EventHandler<WorkerEventArgs> WorkCompleted;
        public event EventHandler WorkerStarted;
        public event EventHandler WorkerStopped;
        public event EventHandler WorkerIdling;

        #endregion


        #region public methods

        public void Start()
        {
            if (State != WorkerState.Working)
            {
                WorkerStarted?.Invoke(this, EventArgs.Empty);
                StartWorkInternal();
            }
        }

        public void Stop()
        {
            WorkerStopped?.Invoke(this, EventArgs.Empty);
            ChangeState(WorkerState.Stopped);
        }

        public void Enqueue(params SimpleWork[] workToDo)
        {
            if (workToDo == null) throw new ArgumentNullException(nameof(workToDo));

            foreach (var work in workToDo)
            {
                _internalQueue.Enqueue(work);
            }
        }

        public void ResetQueue(SimpleWork workToDo)
        {
            _internalQueue.Clear();
        }

        #endregion


        #region private methods

        private void DoWork()
        {
            while (State!= WorkerState.Stopped)
            {
                while (State != WorkerState.Stopped && _internalQueue.TryDequeue(out var work))
                {
                    State = WorkerState.Working;
                    var workerEventArgs = new WorkerEventArgs {WorkName = work.Name, WorkDuration = work.DurationSeconds};

                    WorkStarting?.Invoke(this, workerEventArgs);

                    Thread.Sleep(work.DurationSeconds * 1000);

                    WorkCompleted?.Invoke(this, workerEventArgs);
                }

                if (State != WorkerState.Stopped)
                {
                    WorkerIdling?.Invoke(this, EventArgs.Empty);
                    ChangeState(WorkerState.Idle);
                }
                Thread.Sleep(100); 
            }
        }

        private void ChangeState(WorkerState newState)
        {
            lock (_stateLock)
            {
                State = newState;
            }
        }

        private void StartWorkInternal()
        {
            ChangeState(WorkerState.Working);

            Task.Run((Action) DoWork); // или Task.Factory.StartNew(DoWork);
        }

        #endregion
    }
}