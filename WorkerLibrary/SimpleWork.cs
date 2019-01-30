using System;
using System.Threading;
using WorkerLibrary.Interfaces;

namespace WorkerLibrary
{
    public class SimpleWork : IWork
    {
        public SimpleWork(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));

            Name = name;
        }

        public bool Completed { get; private set; }
        public string Name { get; }

        public void DoWork()
        {
            Console.WriteLine($"Производится работа {Name}");
            Thread.Sleep(2000);
            Completed = true;
        }
    }
}