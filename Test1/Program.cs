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
            MainWriteLine("Нагружаем группой заданий 1.");

            MainWriteLine($"Начали ставить в очередь {firstBatch.Length} заданий.");
            worker.Enqueue(firstBatch);
            MainWriteLine($"Закончили ставить в очередь {firstBatch.Length} заданий.\r\n");

            MainWriteLine("Запускаем работника.");
            worker.Start();

            var secondBatch = GetWorkData(2);
            MainWriteLine("Нагружаем группой заданий 2");

            MainWriteLine($"Начали ставить в очередь {secondBatch.Length} заданий.");
            worker.Enqueue(secondBatch);
            MainWriteLine($"Закончили ставить в очередь {secondBatch.Length} заданий.\r\n");

            //ждем пока закончит
            MainWriteLine("Ждем пока работник разгребет первые две партии.");
            while (worker.State == WorkerState.Working)
            {
                Thread.Sleep(100);
            }

            MainWriteLine("Работник разгрёб первые две партии.");

            var thirdBatch = GetWorkData(2);
            MainWriteLine("Нагружаем группой заданий 3.");

            MainWriteLine($"Начали ставить в очередь {thirdBatch.Length} заданий.");
            worker.Enqueue(thirdBatch);
            MainWriteLine($"Закончили ставить в очередь {thirdBatch.Length} заданий.\r\n");

            Thread.Sleep(1000);
            MainWriteLine("Останавливаем работника...");
            worker.Stop();
            
            //waiting for worker to finish
            MainWriteLine("Ждем пока работник разгребет последнюю партию.");
            while (worker.State == WorkerState.Working)
            {
                Thread.Sleep(100);
            }
            MainWriteLine("Работник закончил с последней партией.");
            
            MainWriteLine("Конец метода Main.");

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
                    Console.WriteLine($"{GetTime()}: Работяга начал работу: {ar.WorkName} с длительностью: {ar.WorkDuration} секунд.");

            worker.WorkCompleted += (obj, ar) =>
                    Console.WriteLine($"{GetTime()}: Работяга закончил работу: {ar.WorkName} с длительностью: {ar.WorkDuration} секунд.\r\n");

            worker.WorkerStarted += (o, a) => Console.WriteLine($"{GetTime()}: Работник запустился.");
            worker.WorkerStopped += (o, a) => Console.WriteLine($"{GetTime()}: Работник остановился.");
            worker.WorkerIdling += (o, a) => Console.WriteLine($"{GetTime()}: Работник простаивает.");
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