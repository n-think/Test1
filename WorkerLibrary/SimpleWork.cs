using System;

namespace WorkerLibrary
{
    public class SimpleWork
    {
        public SimpleWork(int duration, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (duration <= 0) throw new ArgumentOutOfRangeException(nameof(duration));

            DurationSeconds = duration;
            Name = name;
        }
        public int DurationSeconds { get; }
        public string Name { get; }
    }
}