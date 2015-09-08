using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PhysicalSimulations;
using ThinkingClassLibary;
using System.Xml.Serialization;
namespace ClassLibrary2361
{
    public class Field : Generational
    {
        public static int NumThread;
        const int NF = 4500;
        bool ThreadsHaveBeenSet;
        bool End = false;
        public InTime<BleepyBloop>[] Bloops;
        public List<Food> Foods;
        public List<Rock> Rocks;
        public List<Poison> Poisons;
        public int FrameValid;
        Thread[] Hs;
        [XmlIgnore]
        public List<IPhysical<Vector2d>> ThingsCache;

        [XmlIgnore]
        EventWaitHandle[] WaiterK;

        public Field(int NumberOfBloops)
        {
            Rocks = new List<Rock>();
            double s = 80;
            for (double r = 0;r<(Math.PI*2);r+=0.024)
            {
                Rocks.Add(new Rock((Math.Sin(r) * s) + 50, (Math.Cos(r) * s) + 50));
            }
            for(int i = 0;i<64;i++)
            {
                Rocks.Add(new Rock());
            }
        }

        public int Frame
        {
            get;
            set;
        }

        public void SetThreads (int n)
        {
            if (!ThreadsHaveBeenSet)
            {
                SetThreadsWithoutChecking(n);
            }
            else
            {
                Console.Beep();
            }
        }

        private void SetThreadsWithoutChecking(int n)
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
                Hs[ThreadIterator] = new Thread(() => SimulateBloops(Start, End, ThreadIterator));
                Hs[ThreadIterator].Start();
                while (Hs[ThreadIterator].ThreadState != ThreadState.WaitSleepJoin) ;
            }
            ThreadsHaveBeenSet = true;
        }

        public override InTime<LifeForm>[] Lifes
        {
            get
            {
                return (Bloops.Select(r => InTime<LifeForm>.FromList(r.AtFrame.ConvertAll(x => (LifeForm)x))).ToArray());
            }
        }

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
        EventWaitHandle[] waitera;

        public static Field GenerateNewLevel(BleepyBloop[] Olds)
        {
            Field F = InitNewFieldWithOldBleeps(Olds);
            AddNewFoodAndPoison(F, 48);
            UpdateThingChache(F);

            F.SetThreads(NumThread);
            return F;
        }

        public static void UpdateThingChache(Field F)
        {
            List<IPhysical<Vector2d>> J = new List<IPhysical<Vector2d>>();
            J.AddRange(F.Bloops.Select(R => R.AtFrame[F.Frame]));
            foreach (Food B in F.Foods)
            {
                J.Add(B);
            }
            foreach (Poison B in F.Poisons)
            {
                J.Add(B);
            }
            F.ThingsCache = J;
            F.FrameValid = F.Frame;
        }

        public static void AddNewFoodAndPoison(Field F, int n)
        {
            for (int i = 0; i < n; i++)
            {
                F.Foods.Add(new Food());
                F.Foods[i] = new Food();
            }
            for (int i = 0; i < n; i++)
            {
                F.Poisons.Add(new Poison());
                F.Poisons[i] = new Poison();
            }
        }

        private static Field InitNewFieldWithOldBleeps(BleepyBloop[] Olds)
        {
            Field F = new Field(Olds.Count());
            F.Bloops = new InTime<BleepyBloop>[Olds.Count()];
            F.Foods = new List<Food>();
            F.Poisons = new List<Poison>();
            for (int i = 0; i < Olds.Count(); i++)
            {
                F.Bloops[i] = new InTime<BleepyBloop>();
                F.Bloops[i].AtFrame.Add(new BleepyBloop());
                F.Bloops[i].AtFrame[0].Genes = Olds[i].Genes;
                F.Bloops[i].AtFrame[0].Food += Olds[i].Food;
            }

            return F;
        }

        public void CAP(Vector2d Point, int Frame, ref ColourVector ot)
        {
            int l = Things(Frame).Count();
            for (int i = 0; i < l; i++)
            {

                IPhysical<Vector2d> Current = Things(Frame)[i];
                if (((Current.Position.x - Point.x) != 0) && (Current.Position.y - Point.y) != 0 && (Current.Position.x - Point.x) < 32 && (Current.Position.y - Point.y) < 32 && (Current.Position.x - Point.x) > -32 && (Current.Position.y - Point.y) > -32)
                {
                    ot.AddMultiply(Current.Colour, 5184 * (Current.Position - Point).inverseQuartic);
                }
            }
            for (int i = 0; i < ot.dimensions; i++)
            {
                ot.CoOrdinates[i] += BetterRandom.StdDev(0.01);
            }
        }
        public void MoveFromBrainToPhy(int Frame)
        {
            for (int i = 0; i < Bloops.Count(); i++)
            {
                BleepyBloop h = Bloops[i].AtFrame[Frame];
                double V = h.MoveSpeed;
                var LThrust = Clamper.clamp(h.Outputs[(int)BleepyBloop.OAL.OutThrustL]);
                var RThrust = Clamper.clamp(h.Outputs[(int)BleepyBloop.OAL.OutThrustR]);
                double T = V * (LThrust + RThrust);
                h.pos.x += T * Math.Sin(h.Rotation);
                h.pos.y += T * Math.Cos(h.Rotation);
                if (double.IsNaN(h.pos.x)) Console.Beep();
                if (double.IsNaN(h.pos.y)) Console.Beep();
                h.Rotation += (RThrust - LThrust) / 5d;
            }

        }
        public void MoveFromPhyToBrain(int Frame)
        {
            for (int i = 0; i < Bloops.Count(); i++)
            {
                BleepyBloop h = Bloops[i].AtFrame[Frame];
                Bloops[i].AtFrame[Frame].L = new ColourVector();
                Bloops[i].AtFrame[Frame].R = new ColourVector();
                Bloops[i].AtFrame[Frame].F = new ColourVector();
                CAP(h.PosSensor(1d, 0.8), Frame, ref Bloops[i].AtFrame[Frame].L);
                CAP(h.PosSensor(-1d, 0.8), Frame, ref Bloops[i].AtFrame[Frame].R);
                CAP(h.PosSensor(0, 1.6), Frame, ref Bloops[i].AtFrame[Frame].F);
                for (int j = 0; j < 4; j++)
                {
                    h.Inputs[j * 3] = Bloops[i].AtFrame[Frame].L.CoOrdinates[j];
                    h.Inputs[j * 3 + 1] = Bloops[i].AtFrame[Frame].R.CoOrdinates[j];
                    h.Inputs[j * 3 + 2] = Bloops[i].AtFrame[Frame].F.CoOrdinates[j];
                }
            }
        }
        public void NullCheck(int Frame)
        {
            foreach (BleepyBloop b in Bloops.Select(x => x.AtFrame[Frame]))
            {
                if (double.IsNaN(b.pos.x) || double.IsNaN(b.pos.y))
                {
                    Console.Beep();
                }
            }
        }
        public void SimulateAllBloops()
        {
            for (int i = 0; i < Bloops.Count(); i++)
            {
                Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstOne] = 1d;
                Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstZero] = 0d;
                Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstNegativeOne] = -1d;
                Bloops[i].AtFrame[Frame].Inputs[(int)BleepyBloop.IAL.ConstHalf] = 0.5d;
                foreach (Instruction I in Bloops[i].AtFrame[Frame].Genes)
                {
                    Bloops[i].AtFrame[Frame].SetAddr(I.OutAdr,
                        I.Instuct.Evaluate(
                            Bloops[i].AtFrame[Frame].GetAddr(I.InAdrA),
                            Bloops[i].AtFrame[Frame].GetAddr(I.HyAdrB)));
                }
            }
        }
        public void SimulateBloops(int start, int end, int K)
        {
            while (Frame < NF)
            {
                if (WaiterA[K] == null) Console.Beep();
                WaiterA[K].WaitOne();
                if (!End)
                {
                    SimulateBrainsOfBloops(start, end);
                }
                else
                {
                    Console.Beep();
                    break;
                }
                WaiterK[K].Set();
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

        public void SimulateBrain(int Frame)
        {
            //t1d.Set();
            //t2d.Set();
            //t3d.Set();
            //t4d.Set();
            //while (Hs.Where(t => t.ThreadState == ThreadState.Running).Count() != 0) ;

            //SimulateAllBloops();
            for (int i = 0; i < WaiterK.Count(); i++)
            {
                WaiterA[i].Set();
            }
            for(int i=0;i< WaiterK.Count();i++)
            {
                WaiterK[i].WaitOne();
            }

        }
        public void SimulatePhy(int Frame)
        {
            for (int i = 0; i < Bloops.Count(); i++)
            {
                ColideCollisions(Frame, i);
                EatAvailibleFood(Frame, i);
                RemovePoison(Frame, i);
                EatAvailiblePoison(Frame, i);
            }

            //create new food

            int PoisonVariable = Field.PoisonVariable(0.01);
            for (int i = 0; i < PoisonVariable; i++)
            {
                InTime<Food> tmp = new InTime<Food>();
                Foods.Add(new Food());
            }

            PoisonVariable = Field.PoisonVariable(0.015);
            for (int i = 0; i < PoisonVariable; i++)
            {
                InTime<Poison> tmp = new InTime<Poison>();
                Poisons.Add(new Poison());
            }

        }

        private void RemovePoison(int Frame, int i)
        {
            if (Bloops[i].AtFrame[Frame].Poison > 0)
            {
                Bloops[i].AtFrame[Frame].Poison -= 0.4;
            }
        }

        private void EatAvailibleFood(int Frame, int i)
        {
            for (int oo = 0; oo < Foods.Count(); oo++)
            {
                if (Math.Abs(Bloops[i].AtFrame[Frame].Position.x - Foods[oo].Position.x) < 1.5d)
                {
                    if (Math.Abs(Bloops[i].AtFrame[Frame].Position.y - Foods[oo].Position.y) < 1.5d)
                    {
                        if ((Bloops[i].AtFrame[Frame].Position - Foods[oo].Position).absolute < 1.5)
                        {
                            Bloops[i].AtFrame[Frame].Food += Foods[oo].Size;
                            Foods[oo].Size = 0;
                        }
                    }
                }
            }
        }

        private void EatAvailiblePoison(int Frame, int i)
        {
            for (int oo = 0; oo < Poisons.Count(); oo++)
            {
                var LocalBloop = Bloops[i].AtFrame[Frame];
                if (Math.Abs(LocalBloop.Position.x - Poisons[oo].Position.x) < 1.5d)
                {
                    if (Math.Abs(LocalBloop.Position.y - Poisons[oo].Position.y) < 1.5d)
                    {
                        if ((LocalBloop.Position - Poisons[oo].Position).absolute < 1.5)
                        {
                            LocalBloop.Food = Math.Max(LocalBloop.Food - Poisons[oo].Size,0);
                            LocalBloop.Poison += Poisons[oo].Size;
                            Poisons[oo].Size = 0;
                        }
                    }
                }
            }
        }

        private void ColideCollisions(int Frame, int i)
        {
            for (int oo = 0; oo < Bloops.Count(); oo++)
            {
                if (i < oo)
                {
                    if (Math.Abs(Bloops[i].AtFrame[Frame].Position.x - Bloops[oo].AtFrame[Frame].pos.x) < 2d)
                    {
                        if (Math.Abs(Bloops[i].AtFrame[Frame].Position.y - Bloops[oo].AtFrame[Frame].pos.y) < 2d)
                        {
                            Vector2d Diff = Bloops[oo].AtFrame[Frame].pos - Bloops[i].AtFrame[Frame].pos;
                            Bloops[i].AtFrame[Frame].pos -= 0.5d * Diff;
                            Bloops[oo].AtFrame[Frame].pos -= -0.5d * Diff;
                        }
                    }
                }
            }
            foreach (Rock r in Rocks)
            {
                if (Math.Abs(Bloops[i].AtFrame[Frame].Position.x - r.Position.x) < 2d)
                {
                    if (Math.Abs(Bloops[i].AtFrame[Frame].Position.y - r.Position.y) < 2d)
                    {
                        Vector2d Diff = Bloops[i].AtFrame[Frame].pos - r.Position;
                        Bloops[i].AtFrame[Frame].pos -= -1d * Diff;
                    }
                }
            }
        }

        private static int PoisonVariable(double Lambda)
        {
            // Algorithm due to Donald Knuth, 1969.
            double p = 1.0, L = Math.Exp(-Lambda);
            int k = 0;
            do
            {
                k++;
                p *= ThinkingClassLibary.BetterRandom.NextDouble();
            }
            while (p > L);
            int PoisonVariable = k - 1;
            return PoisonVariable;
        }

        public override bool Step()
        {
            //NullCheck(Frame);
            if ((Frame % 4) == 0)
            {
                MoveFromPhyToBrain(Frame);
            }
            //NullCheck(Frame);
            SimulateBrain(Frame);
            //NullCheck(Frame);
            MoveFromBrainToPhy(Frame);
            //NullCheck(Frame);
            SimulatePhy(Frame);
            //NullCheck(Frame);
            for (int i = 0; i < Bloops.Count(); i++)
            {
                Bloops[i].AtFrame.Add(Bloops[i].AtFrame.Last());
            }
            Frame++;
            if(Frame>NF)
            {
                for(int i=0;i<Hs.Count();i++)
                {
                    WaiterA[i].Set();
                    Hs[i].Join();
                }
            }
            return (Frame > NF);
        }
        public List<IPhysical<Vector2d>> Things(int Frame)
        {
            if (Frame != FrameValid)
            {
                UpdateThingCache(Frame);
            }
            return ThingsCache;
            
        }

        private void UpdateThingCache(int Frame)
        {
            List<IPhysical<Vector2d>> J = new List<IPhysical<Vector2d>>();
            J.AddRange(Bloops.Select(R => R.AtFrame[Frame]));
            foreach (Food F in Foods)
            {
                if (F.Size > 0)
                {
                    J.Add(F);
                }
            }
            foreach (Rock R in Rocks)
            {
                J.Add(R);
            }
            ThingsCache = J;
            FrameValid = Frame;
        }

        private static InTime<BleepyBloop>[] GenerateNewGeneration(LifeForm[] Olds)
        {
            InTime<BleepyBloop>[] F = new InTime<BleepyBloop>[Olds.Count()];
            Olds = Olds.OrderByDescending(y => y.ObjectiveFunction()).Take((int)(Olds.Count() / 2)).ToArray();
            F = new InTime<BleepyBloop>[Olds.Count() * 2];
            for (int i = 0; i < Olds.Count() * 2; i++)
            {
                F[i].AtFrame = new List<BleepyBloop>();
                F[i].AtFrame.Add(new BleepyBloop());
                F[i].AtFrame[0].Genes = Olds[i % Olds.Count()].Genes;
            }
            for (int i = Olds.Count(); i < Olds.Count() * 2; i++)
            {
                F[i].AtFrame[0].Vary(0.08, 0.08);
            }
            return F;
        }

        public Field ()
        {
        }
    }
}
