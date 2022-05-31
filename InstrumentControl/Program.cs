using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InstrumentControl 
{
    class Program
    {
        private List<NumberAverager> processedData = new List<NumberAverager>();
        public List<NumberAverager> ProcessedData { get { return this.processedData; } set { this.processedData = value; } }

        static void Main(string[] args)
        {
            // for running from console one of many app options: 
            // swithc(args)
            // case 1: App1
            // case 2: App2



            Program program = new Program();
            List<NumberAverager> processedAverager = new();
            ProgramHelpers prg = new(5);
            Random rand = new Random(1551);
            prg.ThresholdReached += program.Prg_ThresholdReached;
            int i = 0;
            while (i < 100)
            {
                prg.AddValueToQueue(ValueGenerator.CreateValue(5, rand));
                if (i % 5 == 0) Console.WriteLine("fifth val generated");
                i++;
            }
            Console.WriteLine();
            Console.WriteLine("Averages:");
            foreach (NumberAverager nAvg in program.ProcessedData)
            {
                Console.WriteLine(nAvg.Average.ToString());
            }
        }
        private async void Prg_ThresholdReached(object? sender, ThresholdReachedEventArgs e)
        {
            var t = Task.Run(() =>
            {
                ProcessedData.Add(new NumberAverager(e.Data));
            });
            await Task.Yield();
            await t;
            Console.WriteLine("Scan averaged!");
        }
        private void Prg_ThresholdNonAsync(object? sender, ThresholdReachedEventArgs e)
        {
            ProcessedData.Add(new NumberAverager(e.Data));
            Console.WriteLine("Scan averaged!");
        }
    }
    public static class ValueGenerator
    {
        public static double CreateValue(int millisecondDelay, Random rnd)
        {
            //Thread.Sleep(millisecondDelay);
            double value = rnd.NextDouble();
            //Console.WriteLine(value.ToString());
            return value;
        }
    }
}