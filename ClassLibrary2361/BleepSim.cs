﻿using System;
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
        public InTime<BleepyBloop>[] Bloops;
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
                if (Hs[ThreadIterator] != null) Hs[ThreadIterator].Join();
                WaiterA[ThreadIterator] = new AutoResetEvent(false);
                WaiterK[ThreadIterator] = new AutoResetEvent(false);
                int Start = (int)Math.Round((ThreadIterator * (double)Bloops.Count() / n));
                int End = (int)Math.Round(((ThreadIterator + 1) * (double)Bloops.Count() / n));
                Hs[ThreadIterator] = new Thread(() => SimulateBloops(Start, End, ThreadIterator,NF));
                Hs[ThreadIterator].Start();
                while (Hs[ThreadIterator].ThreadState != ThreadState.WaitSleepJoin) ;
            }
            ThreadsHaveBeenSet = true;
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

                foreach (Instruction I in Bloops[i].AtFrame[Frame].Genes)
                {
                    Bloops[i].AtFrame[Frame].SetAddr(I.OutAdr,
                        I.Instuct.Evaluate(
                            Bloops[i].AtFrame[Frame].GetAddr(I.InAdrA),
                            Bloops[i].AtFrame[Frame].GetAddr(I.HyAdrB)));
                }
            }
        }
        private void AccessBuffer(int i)
        {
            for (int j = 0; j < Bloops[i].AtFrame[Frame].Buffet.Count(); j++)
            {
                Bloops[i].AtFrame[Frame].Buffet[j].Set(Bloops[i].AtFrame[Frame].Outputs[j * 3 + 3], Bloops[i].AtFrame[Frame].Outputs[(j * 3) + 2]);
                Bloops[i].AtFrame[Frame].Inputs[16 + j] = Bloops[i].AtFrame[Frame].Buffet[j].Get(Bloops[i].AtFrame[Frame].Outputs[j * 3 + 4]);
            }
        }
        private void SetConstants(int i)
        {
            Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstOne] = 1d;
            Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstZero] = 0d;
            Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstNegativeOne] = -1d;
            Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstHalf] = 0.5d;
        }
    }
}