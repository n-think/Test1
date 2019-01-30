using System;

namespace WorkerLibrary
{
    public class WorkerEventArgs : EventArgs
    {
        public string WorkName;
        public int WorkDuration;
    }
}