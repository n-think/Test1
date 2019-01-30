using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WorkerLibrary.Enums;
using WorkerLibrary.Interfaces;

namespace WorkerLibrary
{
    public sealed class Worker
    {
        public Worker(ConcurrentQueue<IWork> queue)
        {
            WorkerQueue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        #region properties & fields

        public ConcurrentQueue<IWork> WorkerQueue { get; }

        public WorkerState State { get; private set; } = WorkerState.Stopped;

        private readonly object _stateLock = new object();

        private bool _stopRequested = false;

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

            _stopRequested = false;
        }

        public void Stop()
        {
            WorkerStopped?.Invoke(this, EventArgs.Empty);
            _stopRequested = true;
        }

        #endregion


        #region private methods

        private void DoWork()
        {
            while (true)
            {
                while (!_stopRequested && WorkerQueue.TryDequeue(out var work))
                {
                    ChangeState(WorkerState.Working);
                    var workerEventArgs = new WorkerEventArgs {WorkName = work.Name};

                    WorkStarting?.Invoke(this, workerEventArgs);

                    // здесь делаем работу
                    try
                    {
                        work.DoWork();
                    }
                    catch (Exception)
                    {
                        //log?
                        Console.WriteLine($"Ошибка при выполнении {work.Name}.");
                    }


                    WorkCompleted?.Invoke(this, workerEventArgs);
                }

                if (!_stopRequested)
                {
                    WorkerIdling?.Invoke(this, EventArgs.Empty);
                    ChangeState(WorkerState.Idle);
                }
                else
                {
                    ChangeState(WorkerState.Stopped);
                    _stopRequested = false;
                    return;
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