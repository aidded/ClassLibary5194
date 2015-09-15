
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using ThinkingClassLibary;
namespace ClassLibrary2361
{
    public class Simulation<Q> where Q : GenerationSet
    {
        public Q GenerationSet;
    }

    public static class SimuStats
    {
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> @this)
        {
            var enumerators = @this.Select(t => t.GetEnumerator())
                                   .Where(e => e.MoveNext());

            while (enumerators.Any())
            {
                yield return enumerators.Select(e => e.Current);
                enumerators = enumerators.Where(e => e.MoveNext());
            }
        }

        public static double StdDev(double[] Vals)
        {
            double mean = 0;
            foreach(double d in Vals)
            {
                mean += d;
            }
            mean /= Vals.Count();
            double dev = 0;
            foreach (double d in Vals)
            {
                dev += Math.Pow(d- dev,2);
            }
            dev /= Vals.Count();
            return Math.Sqrt(dev);
        }
    }

    public class FieldSimulation
    {
        bool f;
        public DateTime LastFinish;
        private Thread[] hs;
        public double MaxStdDevMeanRatio()
        {
            return SimuStats.Transpose(Turnip.Generations.Select(t => t.Lifes.Select(l => l.ObjectiveFunction()))).Select(c => SimuStats.StdDev(c.ToArray()) / c.Average()).Max();
        }
        public FieldSet Turnip;

        public Thread[] Hs
        {
            get
            {
                return hs;
            }

            set
            {
                hs = value;
            }
        }

        public bool Step(int GenerationCount)
        {
            bool IsNewLevel = Turnip.Generations[Turnip.Generations.Count - 1].Step();
            if (IsNewLevel)
            {
                NewLevel(GenerationCount);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void NewLevel(int GenerationCount)
        {
            int LastGenIndex = Turnip.Generations.Count() - 1;
            int LastFrameIdx = Turnip.Generations[LastGenIndex].Frame;
            //GetLastGenBleepyBloopsFromLastGeneration
            IEnumerable<BleepyBloop> Olds = Turnip.Generations[LastGenIndex].Lifes.Select(j => (BleepyBloop)j);

            FieldSet NewFieldSet = null;
            DateTime Now = DateTime.UtcNow;
            if ((GenerationCount % 6) == 0)
            {
                NewFieldSet = NewGeneration(Olds, Now);
            }
            else
            {
                Turnip.Generations.Add(Field.GenerateNewLevel(Olds.ToArray()));
                if (!f)
                {
                    PrintSpeeds(Olds, Now);
                }
            }
        }

        private FieldSet NewGeneration(IEnumerable<BleepyBloop> Olds, DateTime Now)
        {
            FieldSet NewFieldSet;

            //PrintSpeeds(Olds, Now);
            Console.Write(Olds.Select(j => j.Food).Sum().ToString("000000")+"\t");
            Console.Write(Olds.Select(j => j.Food).Average().ToString("000000") + "\t");
            Console.Write(Olds.Count()+"\t");
            if (f)
            {
                SerializeBloops(Olds);
            }
            Console.WriteLine();
            NewFieldSet = FieldSet.NewField(Olds.ToArray());
            Turnip = NewFieldSet;
            return NewFieldSet;
        }

        private static void SerializeBloops(IEnumerable<BleepyBloop> Olds)
        {
            int i = 0;
            foreach (BleepyBloop B in Olds.OrderByDescending(j => j.ObjectiveFunction()))
            {

                Console.Write(B.ObjectiveFunction().ToString("00000.0") + "\t");
                XmlSerializer x = new XmlSerializer(typeof(Instruction[]));
                TextWriter w = new StreamWriter(@"C:\Users\Admin\Documents\Cout\" + "b_" + B.ObjectiveFunction().ToString("0000") + "_" + i.ToString() + ".xml");
                i++;
                x.Serialize(w, B.Genes);
                w.Close();
            }
        }

        private void PrintSpeeds(IEnumerable<BleepyBloop> Olds, DateTime Now)
        {
            Console.WriteLine();
            Console.Write((((double)Olds.Count() * 1000) / (Now - LastFinish).TotalMilliseconds).ToString("0000.0000") + "\t");
            Console.Write(((Now - LastFinish).TotalMilliseconds / (60000)).ToString("0000.0000") + "\t");
            LastFinish = Now;
        }

        public void init(int NumberOfBlips,bool F)
        {
            f = F;
            Turnip = FieldSet.NewField(NumberOfBlips);
            LastFinish = DateTime.UtcNow;
        }

        public void WriteGeneration()
        {

        }
    }
}
