using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using ThinkingClassLibary;
namespace ClassLibrary2361
{
    public class BleepSim
    {
        public static int NumThread;
        public Thread[] Hs;
        private bool ThreadsHaveBeenSet;
        [XmlIgnore]
        private EventWaitHandle[] waitera;
        [XmlIgnore]
        public EventWaitHandle[] WaiterA
        {
            get
            {
                return waitera;
            }

            set
            {
                waitera = value;
            }
        }
        [XmlIgnore]
        public EventWaitHandle[] WaiterK;
        public BleepyBloop[] Bloops;
        public void SetThreads(int n, int NF)
        {
            if (!ThreadsHaveBeenSet)
            {
                SetThreadsWithoutChecking(n, NF);
            }
            else
            {
                Console.Beep();
            }
        }
        public int Frame;
        public void SimulateBloops(int start, int end, int K, int NF)
        {
            while (Frame < NF)
            {
                if (WaiterA[K] == null) Console.Beep();
                WaiterA[K].WaitOne();
                SimulateBrainsOfBloops(start, end);
                WaiterK[K].Set();
            }
        }
        private void SetThreadsWithoutChecking(int n,int NF)
        {
            Hs = new Thread[n];
            WaiterA = new EventWaitHandle[n];
            WaiterK = new EventWaitHandle[n];
            for (int ThreadIterator = 0; ThreadIterator < n; ThreadIterator++)
            {
                SetIndividualThread(n, NF, ThreadIterator);
            }
            ThreadsHaveBeenSet = true;
        }

        private void SetIndividualThread(int n, int NF, int ThreadIterator)
        {
            if (Hs[ThreadIterator] != null) Hs[ThreadIterator].Join();

            SetWaiters(ThreadIterator);

            SetUpThread(n, NF, ThreadIterator);
            Hs[ThreadIterator].Start();

            while (Hs[ThreadIterator].ThreadState != ThreadState.WaitSleepJoin) ;
        }

        private void SetUpThread(int n, int NF, int ThreadIterator)
        {
            int Start = (int)Math.Round((ThreadIterator * (double)Bloops.Count() / n));
            int End = (int)Math.Round(((ThreadIterator + 1) * (double)Bloops.Count() / n));

            Hs[ThreadIterator] = new Thread(() => SimulateBloops(Start, End, ThreadIterator, NF));
        }

        private void SetWaiters(int ThreadIterator)
        {
            WaiterA[ThreadIterator] = new AutoResetEvent(false);
            WaiterK[ThreadIterator] = new AutoResetEvent(false);
        }

        public void RunThreads()
        {
            for (int i = 0; i < WaiterK.Count(); i++)
            {
                WaiterA[i].Set();
            }
            for (int i = 0; i < WaiterK.Count(); i++)
            {
                WaiterK[i].WaitOne();
            }
        }
        private void SimulateBrainsOfBloops(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                SetConstants(i);

                AccessBuffer(i);

                foreach (Instruction I in Bloops[i].Genes)
                {
                    Bloops[i].SetAddr(I.OutAdr,
                        I.Instuct.Evaluate(
                            Bloops[i].GetAddr(I.InAdrA),
                            Bloops[i].GetAddr(I.HyAdrB)));
                }
            }
        }
        private void AccessBuffer(int i)
        {
            BleepyBloop Bloop = Bloops[i];
            for (int j = 0; j < Bloop.Buffet.Count(); j++)
            {
                Bloop.Buffet[j].Set(Bloop.Outputs[j * 3 + 3], Bloop.Outputs[(j * 3) + 2]);
                Bloop.Inputs[16 + j] = Bloop.Buffet[j].Get(Bloop.Outputs[j * 3 + 4]);
            }
        }
        private void SetConstants(int i)
        {
            Bloops[i].Inputs[(int)BleepyBloop.IAL.ConstOne] = 1d;
            Bloops[i].Inputs[(int)BleepyBloop.IAL.ConstZero] = 0d;
            Bloops[i].Inputs[(int)BleepyBloop.IAL.ConstNegativeOne] = -1d;
            Bloops[i].Inputs[(int)BleepyBloop.IAL.ConstHalf] = 0.5d;
        }
    }
}
