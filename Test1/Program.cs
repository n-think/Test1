using System;
using System.Diagnostics;
using System.Threading;
using WorkerLibrary;
using WorkerLibrary.Enums;

namespace Test1
{
    class Program
    {
        private static int workCounter = 1;
        private static Random rnd = new Random();
        private static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            var worker = Worker.GetInstance;
            stopwatch.Start();

            ConfigureWorker(worker);

            var firstBatch = GetWorkData(2);
            MainWriteLine("Adding first batch.");

            MainWriteLine($"Queueing {firstBatch.Length} tasks.");
            worker.Enqueue(firstBatch);
            MainWriteLine($"Finished queueing {firstBatch.Length} tasks.\r\n");

            MainWriteLine("Starting worker.");
            worker.Start();

            var secondBatch = GetWorkData(2);
            MainWriteLine("Adding second batch.");

            MainWriteLine($"Queueing {secondBatch.Length} tasks.");
            worker.Enqueue(secondBatch);
            MainWriteLine($"Finished queueing {secondBatch.Length} tasks.\r\n");

            //waiting for worker to finish
            MainWriteLine("Waiting for worker to finish first two batches");
            while (worker.State == WorkerState.Working)
            {
                Thread.Sleep(100);
            }

            MainWriteLine("Finished waiting for worker to finish first two batches");

            var thirdBatch = GetWorkData(2);
            MainWriteLine("Adding third batch.");

            MainWriteLine($"Queueing {thirdBatch.Length} tasks.");
            worker.Enqueue(secondBatch);
            MainWriteLine($"Finished queueing {thirdBatch.Length} tasks.\r\n");

            Thread.Sleep(1000);
            MainWriteLine("Stopping worker...");
            worker.Stop();

            MainWriteLine("end of Main.");

            Console.ReadKey();
        }

        private static SimpleWork[] GetWorkData(int count)
        {
            var data = new SimpleWork[count];
            for (var i = 0; i < count; i++)
            {
                data[i] = new SimpleWork(rnd.Next(2, 5), $"work {workCounter++}");
            }

            return data;
        }

        private static void ConfigureWorker(Worker worker)
        {
            if (worker == null) throw new ArgumentNullException(nameof(worker));

            worker.WorkStarting += (obj, ar) =>
                    Console.WriteLine($"{GetTime()}: Worker started work: {ar.WorkName} with duration: {ar.WorkDuration} seconds.");

            worker.WorkCompleted += (obj, ar) =>
                    Console.WriteLine($"{GetTime()}: Worker completed work: {ar.WorkName} with duration: {ar.WorkDuration} seconds.\r\n");

            worker.WorkerStarted += (o, a) => Console.WriteLine($"{GetTime()}: Worker started.");
            worker.WorkerStopped += (o, a) => Console.WriteLine($"{GetTime()}: Worker stopped.");
            worker.WorkerIdling += (o, a) => Console.WriteLine($"{GetTime()}: Worker is idling.");
        }

        private static string GetTime()
        {
            return stopwatch.Elapsed.ToString("ss\\.fff");
        }


        private static void MainWriteLine(string text)
        {
            Console.WriteLine($"{GetTime()} <Main>: {text}");
        }
    }
}