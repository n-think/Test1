using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using WorkerLibrary;
using WorkerLibrary.Enums;
using WorkerLibrary.Interfaces;
using Xunit;

namespace UnitTests
{
    public class UnitTests
    {
        [Fact]
        public void WorkerWorks()
        {
            var worker = new Worker(new ConcurrentQueue<IWork>());
            var work = new StubWork();
            
            worker.WorkerQueue.Enqueue(work);

            worker.Start();

            while (worker.State == WorkerState.Working)
            {
                Thread.Sleep(10);
            }
            
            Assert.True(work.Completed);
        }
    }

    public class StubWork : IWork
    {
        public bool Completed { get; private set; }
        public string Name { get; }

        public void DoWork()
        {
            Completed = true;
        }
    }
}